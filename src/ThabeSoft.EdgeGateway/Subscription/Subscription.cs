using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Readings;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Primitives;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.Protocols;
using ThabeSoft.Triggers;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class Subscription<T>(SubscriptionDependencies<T> dependencies) :
    ISubscription<T>, IAsyncDisposable
    where T : notnull
{
    private DisposeFlag _disposeFlag = new();
    private IDisposable? _triggerTick;


    public SubscriptionId Id { get; } = dependencies.Descriptor.Id;
    public ProtocolAddress Address { get; } = dependencies.Descriptor.Tag.Address;
    public Reading<T>? Latest => dependencies.ReadingSource.Latest;
    Reading? ISubscription.Latest => dependencies.ReadingSource.Latest;
    public IObservable<Reading<T>> Update => dependencies.ReadingObservable;
    IObservable<Reading> ISubscription.Update => dependencies.ReadingObservable;
    public LifecycleState State => dependencies.StateMachine.State;


    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);
        return dependencies.StateMachine.TransitionToRunningAsync(OnStartAsync, cancellationToken);
    }
    public ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);
        return dependencies.StateMachine.TransitionToStoppedAsync(OnStopAsync, cancellationToken);
    }
    public async ValueTask DisposeAsync()
    {
        if (!_disposeFlag.TryMarkDisposed()) return;

        _triggerTick?.TryDispose(dependencies.Logger);


        if (dependencies.OnDispose is null) return;
        try { await dependencies.OnDispose.Invoke(); }
        catch (Exception ex) { dependencies.Logger.LogError(ex, "订阅释放回调异常"); }
    }


    private ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (dependencies.DeviceState.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("设备未启动");

        _triggerTick?.Dispose();
        _triggerTick = dependencies.TriggerScheduler.Register(dependencies.Descriptor.Trigger, ReadingUpdateAsync);

        return ValueTask.CompletedTask;
    }
    private ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        _triggerTick?.Dispose();
        return ValueTask.CompletedTask;
    }

    private async ValueTask ReadingUpdateAsync(CancellationToken cancellationToken)
    {
        if (State is not LifecycleState.Running) return;

        try
        {
            var reading = await dependencies.ReadingSource.ReadAsync(cancellationToken);
            dependencies.ReadingObservable.OnNext(reading);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _triggerTick?.TryDispose(dependencies.Logger);
            _triggerTick = null;
        }
        catch (Exception ex)
        {
            dependencies.ReadingObservable.OnError(ex);

            _triggerTick?.TryDispose(dependencies.Logger);
            _triggerTick = null;
        }
    }
}

internal sealed class SubscriptionDependencies<T> where T : notnull
{
    // Device
    public required ILifecycleStateable DeviceState { get; init => field = value.Required(); }

    // Subscription
    public required SubscriptionDescriptor<T> Descriptor { get; init => field = value.Required(); }
    public required ReadingSource<T> ReadingSource { get; init => field = value.Required(); }
    public required ReadingObservable<T> ReadingObservable { get; init => field = value.Required(); }
    public required ILifecycleStateMachine StateMachine { get; init => field = value.Required(); }


    // Services
    public required ITriggerScheduler TriggerScheduler { get; init => field = value.Required(); }
    public required Func<ValueTask> OnDispose { get; init => field = value.Required(); }
    public required ILogger<Subscription<T>> Logger { get; init => field = value.Required(); }
}