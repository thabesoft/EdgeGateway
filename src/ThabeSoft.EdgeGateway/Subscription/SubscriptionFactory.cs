using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Readings;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.Primitives;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class SubscriptionFactory(
        ILifecycleStateMachineFactory lifecycleStateMachineFactory,
        ITriggerScheduler triggerScheduler,
        IEventPublisher eventPublisher,
        ILoggerFactory loggerFactory
    )
{
    public Subscription<T> Create<T>(in SubscriptionCreationArgs<T> context) where T : notnull
    {
        // 日志
        var subscriptionLogger = loggerFactory.CreateLogger<Subscription<T>>();
        var readingSourceLogger = loggerFactory.CreateLogger<ReadingSource<T>>();
        var readingObservableLogger = loggerFactory.CreateLogger<ReadingObservable<T>>();

        var portId = context.Port.Id;
        var deviceId = context.Device.Id;
        var subscriptionId = context.Subscription.Id;

        // 状态机
        var stateMachine = lifecycleStateMachineFactory.Create(context.SubscriptioStateMachineDescriptor);
        var subscriptionStateEventDisposable = stateMachine.StateChanged.Subscribe(x =>
                _ = eventPublisher.PublishAsync(new SubscriptionStateChanged(portId, deviceId, subscriptionId, x.Old, x.New)));

        // 数据更新
        var readingSource = new ReadingSource<T>(context.Subscription.Tag, context.ProtocolAccessor, readingSourceLogger);
        var readingObservable = new ReadingObservable<T>(readingObservableLogger);

        // 构造依赖
        var devicArgs = new SubscriptionArgs<T>()
        {
            DeviceState = context.DeviceState,
            Subscription = context.Subscription,
            StateMachine = stateMachine,
            ReadingObservable = readingObservable,
            ReadingSource = readingSource,
            TriggerScheduler = triggerScheduler,
            Logger = subscriptionLogger,
            DisposeCallback = async ()=>
            {
                await Safe.DisposeSafelyAsync(subscriptionStateEventDisposable, subscriptionLogger);
                await Safe.DisposeSafelyAsync(readingObservable, readingObservableLogger);
                await Safe.DisposeSafelyAsync(readingSource, readingSourceLogger);
                await Safe.DisposeSafelyAsync(stateMachine, subscriptionLogger);
            }
        };

        // 创建
        return new(devicArgs);
    }
}

internal readonly struct SubscriptionCreationArgs<T> where T : notnull
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