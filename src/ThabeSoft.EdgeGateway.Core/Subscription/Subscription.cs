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
        ILogger<Subscription<T>>? logger = null
    ) : ISubscription<T>, IAsyncDisposableSubscription
    where T : notnull
{
    private IDisposable? _triggerTick;
    private IStrictLifecycleStateMachine _stateMachine = Lifecycle.StrictStateMachine(logger);
    private readonly ReadingSubject<T> _subject = new(logger);


    public SubscriptionId Id { get; } = subscriptionDescriptor.Id;
    public IDataAddress Address { get; } = subscriptionDescriptor.Tag.Address;
    public Reading<T>? Latest => subscriptionReader.Latest;
    Reading? ISubscription.Latest => subscriptionReader.Latest;
    public IObservable<Reading<T>> Update => _subject;
    IObservable<Reading> ISubscription.Update => _subject;
    public LifecycleState State => _stateMachine.State;



    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);


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
        catch (Exception ex)
        {
            _subject.OnError(ex);
            throw;
        }
    }
}



internal sealed class SubscriptionReader<T>(
    SubscriptionDescriptor<T> descriptor,
    IProtocolAccessor protocolAccessor,
    RecoveryPolicyHandler recoveryPolicyHandler) where T : notnull
{
    public Reading<T>? Latest { get; private set; }

    public async ValueTask<Reading<T>> ReadAsync(CancellationToken ct)
    {
        try
        {
            var value = await protocolAccessor.ReadAsync(descriptor.Tag, ct);
            var reading = Reading.Good(value);
            Latest = reading;

            return reading;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex) when (recoveryPolicyHandler.Handle(ex))
        {
            return Reading.Bad(GetLatestValue());
        }
    }

    private T GetLatestValue() => Latest is null ? default! : Latest.Value.Value;
}