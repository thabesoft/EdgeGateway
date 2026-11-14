using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Recoveries;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Subscription;


internal interface IAsyncDisposableSubscription : ISubscription, IAsyncDisposable;

internal sealed class Subscription<T>(
        SubscriptionDescriptor<T> descriptor,
        ILifecycleStateMachine lifecycleStateMachine,
        ILifecycleStateable deviceLifecycleState,
        ITriggerScheduler triggerScheduler,
        SubscriptionReader<T> reader,
        ILogger<Subscription<T>>? logger = null
    ) : ISubscription<T>, IAsyncDisposableSubscription
    where T : notnull
{
    private DisposeNotifier _disposeNotifier = new(logger);
    private IDisposable? _triggerTick;
    private readonly ReadingSubject<T> _subject = new(logger);


    public SubscriptionId Id { get; } = descriptor.Id;
    public IDataAddress Address { get; } = descriptor.Tag.Address;
    public Reading<T>? Latest => reader.Latest;
    Reading? ISubscription.Latest => reader.Latest;
    public IObservable<Reading<T>> Update => _subject;
    IObservable<Reading> ISubscription.Update => _subject;
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
        if (deviceLifecycleState.State != LifecycleState.Running)
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
        _subject.TryDispose(logger);

        await _disposeNotifier.RaiseAsync(this);
        await _disposeNotifier.TryDisposeAsync(logger);
    }


    private async ValueTask UpdateAsync(CancellationToken cancellationToken)
    {
        if (State is not LifecycleState.Running) return;

        try
        {
            var reading = await reader.ReadAsync(cancellationToken);
            _subject.OnNext(reading);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {

        }
        catch (Exception ex)
        {
            _subject.OnError(ex);

            _triggerTick?.TryDispose(logger);
            _triggerTick = null;
        }
    }
}