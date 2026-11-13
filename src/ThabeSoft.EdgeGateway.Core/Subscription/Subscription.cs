using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Subscription;


internal interface IAsyncDisposeSubscription : ISubscription, IAsyncDisposable;

internal sealed class Subscription<T> : ISubscription<T>, IAsyncDisposeSubscription
    where T : notnull
{
    public delegate ValueTask<T> GetValueHandler(CancellationToken cancellationToken = default);

    private SubscriptionValueReader<T> _subscriptionValueReader;
    private readonly ConcurrentDictionary<Guid, IObserver<Reading<T>>> _observers = [];

    private readonly SubscriptionDescriptor<T> _subscriptionDescriptor;
    private readonly ILogger<Subscription<T>> _logger;


    private readonly ILifecycleStateable _deviceLifecycleState;
    private readonly IDisposable _triggerReleaser;
    private readonly IRetryableLifecycleStateMachine _stateMachine;


    public Subscription(
            SubscriptionDescriptor<T> subscriptionDescriptor,
            ITriggerScheduler triggerScheduler,
            IProtocolAccessor protocolAccessor,
            ILifecycleStateable deviceLifecycleState,
            ILogger<Subscription<T>>? logger = null)
    {
        _logger = logger ?? NullLogger<Subscription<T>>.Instance;
        _subscriptionDescriptor = subscriptionDescriptor;
        _deviceLifecycleState = deviceLifecycleState;

        _triggerReleaser = triggerScheduler.Register(subscriptionDescriptor.Trigger, TriggerTask);

        _subscriptionValueReader = new(subscriptionDescriptor, protocolAccessor);
        _stateMachine = Lifecycle.RetryableStateMachine(logger);
    }


    public SubscriptionId Id => _subscriptionDescriptor.Id;
    public IDataAddress Address => _subscriptionDescriptor.Tag.Address;
    public Reading<T>? Latest { get; private set; }
    Reading? ISubscription.Latest => Latest;
    public LifecycleState State => _stateMachine.State;


    public IDisposable Subscribe(IObserver<Reading<T>> observer)
    {
        return _stateMachine.Execute(() =>
        {
            var id = Guid.NewGuid();
            _observers[id] = observer;

            return Disposer.From(() => _observers.TryRemove(id, out _));
        });
    }
    public IDisposable Subscribe(IObserver<Reading> observer)
    {
        return Subscribe(new ObserverWapper(observer));
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        return _stateMachine.StartAsync(async ct =>
        {
            if (_deviceLifecycleState.State is not (LifecycleState.Starting or LifecycleState.Running))
                throw new InvalidOperationException("订阅启动失败, 设备未启动");
        }, cancellationToken);
    }
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(_ => ValueTask.CompletedTask, cancellationToken);
    public ValueTask DisposeAsync()
    {
        return _stateMachine.DisposeAsync(async () =>
        {
            _triggerReleaser.Dispose();

            OnCompleted();
            _observers.Clear();
        });
    }




    private async ValueTask TriggerTask(CancellationToken cancellationToken)
    {
        if (State is not LifecycleState.Running) return;
        await OnPublishAsync(cancellationToken);
    }
    private async ValueTask OnPublishAsync(CancellationToken cancellationToken)
    {
        try
        {
            var value = await _subscriptionValueReader.WaitNextAsync(cancellationToken);

            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace("订阅 [{Id}] 新值 [{Value}] 已发布", Id, value);

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
            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace(ex, "订阅 [{Id}] 异常已发布", Id);

            OnNext(Reading.Bad(Latest!.Value.Value));
            //OnError(ex);
            //throw;
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

    private sealed class ObserverWapper(IObserver<Reading> observer) : IObserver<Reading<T>>
    {
        public void OnNext(Reading<T> value) => observer.OnNext(value);
        public void OnCompleted() => observer.OnCompleted();
        public void OnError(Exception error) => observer.OnError(error);
    }
}