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


internal interface IAsyncDisposableSubscription : ISubscription, IAsyncDisposable
{
    ValueTask UpdateAsync(CancellationToken cancellationToken);
}

internal sealed class Subscription<T>(
        SubscriptionDescriptor<T> subscriptionDescriptor,
        ILifecycleStateable deviceLifecycleState,
        RecoveryPolicyHandler recoveryPolicyHandler,
        IProtocolAccessor protocolAccessor,
        ITriggerScheduler triggerScheduler,
        ILogger<Subscription<T>>? logger = null
    ) : ISubscription<T>, IAsyncDisposableSubscription
    where T : notnull
{
    private IDisposable? _triggerTick;
    private readonly ReadingSubject<T> _subject = new(logger);
    private readonly ILogger<Subscription<T>> _logger = logger ?? NullLogger<Subscription<T>>.Instance;


    public SubscriptionId Id { get; } = subscriptionDescriptor.Id;
    public IDataAddress Address { get; } = subscriptionDescriptor.Tag.Address;
    public Reading<T>? Latest { get; private set; }
    Reading? ISubscription.Latest => Latest;
    public IObservable<Reading<T>> Update => _subject;
    IObservable<Reading> ISubscription.Update => _subject;
    public LifecycleState State { get; private set; }



    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        if (deviceLifecycleState.State != LifecycleState.Running)
            return ValueTask.CompletedTask;

        _triggerTick?.Dispose();
        _triggerTick = triggerScheduler.Register(subscriptionDescriptor.Trigger, UpdateAsync);

        State = LifecycleState.Running;
        return ValueTask.CompletedTask;
    }
    public ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        _triggerTick?.Dispose();
        State = LifecycleState.Stopped;
        return ValueTask.CompletedTask;
    }
    public ValueTask DisposeAsync()
    {
        _subject.Dispose();
        State = LifecycleState.Disposed;
        return ValueTask.CompletedTask;
    }

    public async ValueTask UpdateAsync(CancellationToken cancellationToken)
    {
        if (State is not LifecycleState.Running) return;

        try
        {
            var value = await protocolAccessor.ReadAsync(subscriptionDescriptor.Tag, cancellationToken);

            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace("订阅 [{Id}] 新值 [{Value}] 已发布", Id, value);

            var reading = Reading.Good(value);
            Latest = reading;
            _subject.OnNext(reading);

            return;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {

        }
        catch (OperationCanceledException)
        {
            _subject.OnNext(Reading.Bad(GetLatestValue()));
        }
        catch (Exception ex)
        {
            bool canRecover;
            try { canRecover = recoveryPolicyHandler.Handle(ex); }
            catch (Exception handlerEx)
            {
                _logger.LogCritical(handlerEx, "异常处理器执行失败！原异常: {OriginalEx}", ex.Message);
                canRecover = false;
            }

            // 不可恢复的异常
            if (!canRecover)
            {
                _subject.OnError(ex);
                throw;
            }

            // 可恢复的
            _subject.OnNext(Reading.Bad(GetLatestValue()));
        }

        // 最新的值
        T GetLatestValue() => Latest is null ? default! : Latest.Value.Value;
    }
}
