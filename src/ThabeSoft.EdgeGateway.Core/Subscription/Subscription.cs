using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class Subscription<T>(
        SubscriptionDescriptor<T> descriptor,
        ITrigger trigger,
        Subscription<T>.GetValueHandler getValueHandler,
        ILogger<Subscription<T>> logger
    ) : LifecycleStateMachine, ISubscription<T> where T : notnull
{
    public delegate ValueTask<T> GetValueHandler(CancellationToken cancellationToken = default);


    private Task? _loopTask;
    private CancellationTokenSource? _loopCts;


    private readonly ConcurrentDictionary<Guid, IObserver<Reading<T>>> _observers = [];



    public SubscriptionId Id => descriptor.Id;
    public IDataAddress Address => descriptor.Tag.Address;
    public Reading<T>? Latest { get; private set; }
    Reading? ISubscription.Latest => Latest;


    public IDisposable Subscribe(IObserver<Reading<T>> observer)
    {
        var id = Guid.NewGuid();
        _observers[id] = observer;
        return new Disposer(() => _observers.TryRemove(id, out _));
    }


    protected override ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (_loopTask is { IsCompleted: false })
            return ValueTask.CompletedTask;

        _loopCts = new CancellationTokenSource();
        _loopTask = TriggerTask(_loopCts.Token);
        return ValueTask.CompletedTask;
    }
    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        if (_loopCts is null || _loopTask is null)
            return;

        await _loopCts.CancelAsync();

        try { await _loopTask; }
        catch (OperationCanceledException) { }
        catch (Exception ex) { logger.LogError(ex, "触发器任务停止失败"); }

        _loopCts.Dispose();
        _loopCts = null;
        _loopTask = null;
    }
    protected override ValueTask OnDisposeAsync()
    {
        OnCompleted();
        _observers.Clear();

        return ValueTask.CompletedTask;
    }


    private async Task TriggerTask(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!await trigger.WaitForNextTickAsync(cancellationToken))
                return;

            await OnPublishAsync(cancellationToken);
        }
    }
    private async ValueTask OnPublishAsync(CancellationToken cancellationToken)
    {
        try
        {
            var value = await getValueHandler.Invoke(cancellationToken);

            if (logger.IsEnabled(LogLevel.Trace))
                logger.LogTrace("订阅 [{Id}] 新值 [{Value}] 已发布", Id, value);

            var reading = Reading.Good(value);
            Latest = reading;
            OnNext(reading);

            return;
        }
        catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationToken)
        {

        }
        catch (Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Trace))
                logger.LogTrace(ex, "订阅 [{Id}] 异常已发布", Id);

            OnError(ex);
        }
    }

    private void OnNext(Reading<T> value)
    {
        foreach (var observer in _observers.Values)
        {
            try { observer.OnNext(value); }
            catch (OperationCanceledException) { throw; }   // 取消传播
            catch (Exception ex) { /* 记日志，继续 */ }
        }
    }
    private void OnError(Exception exception)
    {
        foreach (var observer in _observers.Values)
        {
            try { observer.OnError(exception); }
            catch (OperationCanceledException) { throw; }   // 取消传播
            catch (Exception ex) { /* 记日志，继续 */ }
        }
    }
    private void OnCompleted()
    {
        foreach (var observer in _observers.Values)
        {
            try { observer.OnCompleted(); }
            catch (OperationCanceledException) { throw; }   // 取消传播
            catch (Exception ex) { /* 记日志，继续 */ }
        }
    }



    private sealed class Disposer(Action action) : IDisposable
    {
        void IDisposable.Dispose() => action.Invoke();
    }
}