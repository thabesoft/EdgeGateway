using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Readings;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Subscription;


internal interface IAsyncDisposableSubscription : ISubscription, IAsyncDisposable;


internal sealed class Subscription<T>(
        SubscriptionDescriptor<T> descriptor,
        ILifecycleStateMachine lifecycleStateMachine,
        ILifecycleStateable deviceLifecycleState,
        ITriggerScheduler triggerScheduler,
        ReadingSource<T> readingSource,
        ILogger<Subscription<T>>? logger = null
    ) : ISubscription<T>, IAsyncDisposableSubscription
    where T : notnull
{
    private IDisposable? _triggerTick;
    private DisposeNotifier _disposeNotifier = new(logger);
    private readonly ReadingObservable<T> _readingObservable = new(logger);


    public SubscriptionId Id { get; } = descriptor.Id;
    public DataAddress Address { get; } = descriptor.Tag.Address;
    public Reading<T>? Latest => readingSource.Latest;
    Reading? ISubscription.Latest => readingSource.Latest;
    public IObservable<Reading<T>> Update => _readingObservable;
    IObservable<Reading> ISubscription.Update => _readingObservable;
    public LifecycleState State => lifecycleStateMachine.State;
    public IDisposeObservable Disposed => _disposeNotifier;



    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        lifecycleStateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        lifecycleStateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        lifecycleStateMachine.DisposeAsync(OnDisposeAsync);


    private ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (deviceLifecycleState.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("设备未启动");

        _triggerTick?.Dispose();
        _triggerTick = triggerScheduler.Register(descriptor.Trigger, UpdateAsync);

        return ValueTask.CompletedTask;
    }
    private ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        _triggerTick?.Dispose();
        return ValueTask.CompletedTask;
    }
    private async ValueTask OnDisposeAsync()
    {
        _triggerTick?.TryDispose(logger);
        _readingObservable.TryDispose(logger);

        await _disposeNotifier.RaiseAsync(this);
        await _disposeNotifier.TryDisposeAsync(logger);
    }


    private async ValueTask UpdateAsync(CancellationToken cancellationToken)
    {
        if (State is not LifecycleState.Running) return;

        try
        {
            var reading = await readingSource.ReadAsync(cancellationToken);
            _readingObservable.OnNext(reading);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _triggerTick?.TryDispose(logger);
            _triggerTick = null;
        }
        catch (Exception ex)
        {
            _readingObservable.OnError(ex);

            _triggerTick?.TryDispose(logger);
            _triggerTick = null;
        }
    }
}