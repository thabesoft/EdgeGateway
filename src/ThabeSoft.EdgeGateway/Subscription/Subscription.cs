using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Readings;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Subscription;


internal interface IAsyncDisposableSubscription : ISubscription, IAsyncDisposable;


internal sealed class Subscription<T>(SubscriptionDependencies<T> dependencies) :
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
    public LifecycleState State => dependencies.StateMachine.State;
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

internal readonly struct SubscriptionDependencies<T> where T : notnull
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



internal sealed class SubscriptionFactory(
        ILifecycleStateMachineFactory lifecycleStateMachineFactory,
        ITriggerScheduler triggerScheduler,
        IEventPublisher eventPublisher,
        ILoggerFactory loggerFactory
    )
{
    public Subscription<T> Create<T>(SubscriptionCreationContext<T> context) where T : notnull
    {
        // 日志
        var subscriptionLogger = loggerFactory.CreateLogger<Subscription<T>>();
        var readingSourceLogger = loggerFactory.CreateLogger<ReadingSource<T>>();
        var readingObservableLogger = loggerFactory.CreateLogger<ReadingObservable<T>>();


        // 状态机
        var stateMachine = lifecycleStateMachineFactory.Create(context.SubscriptioStateMachineDescriptor);
        var subscriptionStateEventDisposable = stateMachine.StateChanged.Subscribe(x =>
                _ = eventPublisher.PublishAsync(new SubscriptionStateChanged(context.Port.Id, context.Device.Id, context.Subscription.Id, x.Old, x.New)));

        // 数据更新
        var readingSource = new ReadingSource<T>(context.Subscription.Tag, context.ProtocolAccessor, readingSourceLogger);
        var readingObservable = new ReadingObservable<T>(readingObservableLogger);

        // 构造依赖
        var devicDependencies = new SubscriptionDependencies<T>()
        {
            DeviceState = context.DeviceState,
            Subscription = context.Subscription,
            StateMachine = stateMachine,
            ReadingObservable = readingObservable,
            ReadingSource = readingSource,
            TriggerScheduler = triggerScheduler,
            Logger = subscriptionLogger
        };

        // 创建
        return new(devicDependencies)
        {
            DisposedCallback = async () =>
            {
                readingObservable.TryDispose(subscriptionLogger);
                subscriptionStateEventDisposable.TryDispose(subscriptionLogger);
                await stateMachine.TryDisposeAsync(subscriptionLogger);
            }
        };
    }
}

internal readonly struct SubscriptionCreationContext<T> where T : notnull
{
    // Port
    public required PortDescriptor Port { get; init => field = value.Required(); }
    public required IProtocolAccessor ProtocolAccessor { get; init => field = value.Required(); }

    // Device
    public required DeviceDescriptor Device { get; init => field = value.Required(); }
    public required ILifecycleStateable DeviceState { get; init => field = value.Required(); }

    // Subscription
    public required SubscriptionDescriptor<T> Subscription { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor SubscriptioStateMachineDescriptor { get; init => field = value.Required(); }
}