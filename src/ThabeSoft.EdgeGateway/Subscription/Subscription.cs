using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Readings;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Subscription;


internal interface IAsyncDisposableSubscription : ISubscription, IAsyncDisposable;


internal sealed class Subscription<T>(in SubscriptionArgs<T> dependencies) :
    ISubscription<T>, IAsyncDisposableSubscription
    where T : notnull
{
    private IDisposable? _triggerTick;
    private readonly ILifecycleStateable _deviceState = dependencies.DeviceState;
    private readonly ITriggerScheduler _triggerScheduler = dependencies.TriggerScheduler;
    private readonly ILifecycleStateMachine _stateMachine = dependencies.StateMachine;
    private readonly ReadingSource<T> _readingSource = dependencies.ReadingSource;
    private readonly ReadingObservable<T> _readingObservable = dependencies.ReadingObservable;
    private readonly TriggerDescriptor _triggerDescriptor = dependencies.Subscription.Trigger;
    private readonly ILogger _logger = dependencies.Logger;



    public SubscriptionId Id { get; } = dependencies.Subscription.Id;
    public DataAddress Address { get; } = dependencies.Subscription.Tag.Address;
    public Reading<T>? Latest => _readingSource.Latest;
    Reading? ISubscription.Latest => _readingSource.Latest;
    public IObservable<Reading<T>> Update => _readingObservable;
    IObservable<Reading> ISubscription.Update => _readingObservable;
    public LifecycleState State => _stateMachine.State;
    public Func<ValueTask>? DisposedCallback { get; init; }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
         _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
         _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
         _stateMachine.DisposeAsync(OnDisposeAsync);


    private ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (_deviceState.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("设备未启动");

        _triggerTick?.Dispose();
        _triggerTick = _triggerScheduler.Register(_triggerDescriptor, UpdateAsync);

        return ValueTask.CompletedTask;
    }
    private ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        _triggerTick?.Dispose();
        return ValueTask.CompletedTask;
    }
    private async ValueTask OnDisposeAsync()
    {
        _triggerTick?.TryDispose(_logger);

        if (DisposedCallback is null) return;

        try { await DisposedCallback.Invoke(); }
        catch (Exception ex) { _logger.LogError(ex, "订阅释放回调异常"); }
    }


    private async ValueTask UpdateAsync(CancellationToken cancellationToken)
    {
        if (State is not LifecycleState.Running) return;

        try
        {
            var reading = await _readingSource.ReadAsync(cancellationToken);
            _readingObservable.OnNext(reading);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _triggerTick?.TryDispose(_logger);
            _triggerTick = null;
        }
        catch (Exception ex)
        {
            _readingObservable.OnError(ex);

            _triggerTick?.TryDispose(_logger);
            _triggerTick = null;
        }
    }
}

internal readonly struct SubscriptionArgs<T> where T : notnull
{
    // Device
    public required ILifecycleStateable DeviceState { get; init => field = value.Required(); }


    public required SubscriptionDescriptor<T> Subscription { get; init => field = value.Required(); }

    public required ReadingSource<T> ReadingSource { get; init => field = value.Required(); }
    public required ReadingObservable<T> ReadingObservable { get; init => field = value.Required(); }
    public required ILifecycleStateMachine StateMachine { get; init => field = value.Required(); }


    public required ITriggerScheduler TriggerScheduler { get; init => field = value.Required(); }
    public required ILogger Logger { get; init => field = value.Required(); }
}