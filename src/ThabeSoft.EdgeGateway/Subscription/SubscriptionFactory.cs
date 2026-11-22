using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.PubSub;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.EdgeGateway.Readings;
using ThabeSoft.Triggers;
using ThabeSoft.Primitives;
using ThabeSoft.Protocols;
using ThabeSoft.EdgeGateway.Messages;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class SubscriptionFactory(
        ILifecycleStateMachineFactory lifecycleStateMachineFactory,
        ITriggerScheduler triggerScheduler,
        IEventPublisher eventPublisher,
        ILoggerFactory loggerFactory
    )
{
    public Subscription<T> Create<T>(SubscriptionCreationArgs<T> args) where T : notnull
    {
        // 日志
        var subscriptionLogger = loggerFactory.CreateLogger<Subscription<T>>();
        var readingSourceLogger = loggerFactory.CreateLogger<ReadingSource<T>>();
        var readingObservableLogger = loggerFactory.CreateLogger<ReadingObservable<T>>();

        // 状态机
        var stateMachine = lifecycleStateMachineFactory.Create(args.Subscription.StateMachine);
        var subscriptionStateEventDisposable = stateMachine.StateChanged.Subscribe(x =>
                _ = eventPublisher.PublishAsync(new SubscriptionStateChangedMessage(args.Port.Id, args.Device.Id, args.Subscription.Id, x.Old, x.New)));

        // 数据更新
        var readingSource = new ReadingSource<T>(args.Subscription.Tag, args.ProtocolAccessor, readingSourceLogger);
        var readingObservable = new ReadingObservable<T>(readingObservableLogger);

        // 构造依赖
        var subscriptionDependencies = new SubscriptionDependencies<T>()
        {
            DeviceState = args.DeviceState,
            Descriptor = args.Subscription,
            StateMachine = stateMachine,
            ReadingObservable = readingObservable,
            ReadingSource = readingSource,
            TriggerScheduler = triggerScheduler,
            Logger = subscriptionLogger,
            OnDispose = async () =>
            {
                await Safe.DisposeSafelyAsync(subscriptionStateEventDisposable, subscriptionLogger);
                await Safe.DisposeSafelyAsync(readingObservable, readingObservableLogger);
                await Safe.DisposeSafelyAsync(readingSource, readingSourceLogger);
                await Safe.DisposeSafelyAsync(stateMachine, subscriptionLogger);
            }
        };

        // 创建
        return new(subscriptionDependencies);
    }
}

internal sealed class SubscriptionCreationArgs<T> where T : notnull
{
    // Port
    public required PortDescriptor Port { get; init => field = value.Required(); }
    public required IProtocolAccessor ProtocolAccessor { get; init => field = value.Required(); }

    // Device
    public required DeviceDescriptor Device { get; init => field = value.Required(); }
    public required ILifecycleStateable DeviceState { get; init => field = value.Required(); }

    // Subscription
    public required SubscriptionDescriptor<T> Subscription { get; init => field = value.Required(); }
}