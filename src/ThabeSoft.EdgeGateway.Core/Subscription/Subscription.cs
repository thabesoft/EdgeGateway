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
        SubscriptionDescriptor<T> subscriptionDescriptor,
        SubscriptionReader<T> subscriptionReader,
        ITriggerScheduler triggerScheduler,
        ILifecycleStateMachine stateMachine,
        ILogger<Subscription<T>>? logger = null
    ) : ISubscription<T>, IAsyncDisposableSubscription
    where T : notnull
{
    private IDisposable? _triggerTick;
    private readonly ReadingSubject<T> _subject = new(logger);


    public SubscriptionId Id { get; } = subscriptionDescriptor.Id;
    public IDataAddress Address { get; } = subscriptionDescriptor.Tag.Address;
    public Reading<T>? Latest => subscriptionReader.Latest;
    Reading? ISubscription.Latest => subscriptionReader.Latest;
    public IObservable<Reading<T>> Update => _subject;
    IObservable<Reading> ISubscription.Update => _subject;
    public LifecycleState State => stateMachine.State;



    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        stateMachine.DisposeAsync(OnDisposeAsync);


    private ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        _triggerTick?.Dispose();
        _triggerTick = triggerScheduler.Register(subscriptionDescriptor.Trigger, UpdateAsync);

        return ValueTask.CompletedTask;
    }
    private ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        _triggerTick?.Dispose();
        return ValueTask.CompletedTask;
    }
    private ValueTask OnDisposeAsync()
    {
        _subject.Dispose();
        return ValueTask.CompletedTask;
    }


    private async ValueTask UpdateAsync(CancellationToken cancellationToken)
    {
        if (State is not LifecycleState.Running) return;

        try
        {
            var reading = await subscriptionReader.ReadAsync(cancellationToken);
            _subject.OnNext(reading);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {

        }
        catch (Exception ex)
        {
            _subject.OnError(ex);

            _triggerTick?.Dispose();
            _triggerTick = null;
        }
    }
}