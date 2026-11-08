using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport.Byte;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class Subscription<T>(
        SubscriptionDescriptor<T> subscriptionDescriptor,
        IProtocolDescriptor protocolDescriptor,
        ITriggerFactorySelector triggerFactorySelector,
        IByteDuplex byteDuplex,
        SubscriptionValueReaderFactory subscriptionValueReaderFactory,
        ILifecycleState deeviceLifecycleState,
        ILogger<Subscription<T>> logger
    ) : ISubscription<T> where T : notnull
{
    public delegate ValueTask<T> GetValueHandler(CancellationToken cancellationToken = default);


    private readonly LifecycleStateMachineA _stateMachine = Lifecycle.Normal(x => x.StartStop(10).Dispose(5), logger: logger);
    private Task? _loopTask;
    private CancellationTokenSource? _loopCts;

    private ITrigger _trigegr = triggerFactorySelector.Create(subscriptionDescriptor.Trigger);
    private SubscriptionValueReader<T> _subscriptionValueReader = subscriptionValueReaderFactory.Create(subscriptionDescriptor, protocolDescriptor, byteDuplex);
    private readonly ConcurrentDictionary<Guid, IObserver<Reading<T>>> _observers = [];



    public SubscriptionId Id => subscriptionDescriptor.Id;
    public IDataAddress Address => subscriptionDescriptor.Tag.Address;
    public Reading<T>? Latest { get; private set; }
    Reading? ISubscription.Latest => Latest;
    public LifecycleState State => _stateMachine.State;


    public IDisposable Subscribe(IObserver<Reading<T>> observer)
    {
        return _stateMachine.Execute(() =>
        {
            var id = Guid.NewGuid();
            _observers[id] = observer;
            return new Disposer(() => _observers.TryRemove(id, out _));
        });
    }
    public IDisposable Subscribe(IObserver<Reading> observer)
    {
        return Subscribe(new Observer(observer));
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
    _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);




    private ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (deeviceLifecycleState.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("订阅启动失败, 设备未启动");

        if (_loopTask is { IsCompleted: false })
            return ValueTask.CompletedTask;

        _loopCts = new CancellationTokenSource();
        _loopTask = TriggerTask(_loopCts.Token);
        return ValueTask.CompletedTask;
    }
    private async ValueTask OnStopAsync(CancellationToken cancellationToken)
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
    private ValueTask OnDisposeAsync()
    {
        OnCompleted();
        _observers.Clear();

        return ValueTask.CompletedTask;
    }


    private async Task TriggerTask(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!await _trigegr.WaitForNextTickAsync(cancellationToken))
                return;

            await OnPublishAsync(cancellationToken);
        }
    }
    private async ValueTask OnPublishAsync(CancellationToken cancellationToken)
    {
        try
        {
            var value = await _subscriptionValueReader.WaitNextAsync(cancellationToken);

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
    private sealed class Observer(IObserver<Reading> observer) : IObserver<Reading<T>>
    {
        public void OnNext(Reading<T> value) => observer.OnNext(value);
        public void OnCompleted() => observer.OnCompleted();
        public void OnError(Exception error) => observer.OnError(error);
    }
}