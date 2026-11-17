using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Readings;
using ThabeSoft.EdgeGateway.Triggers;

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
            Logger = subscriptionLogger
        };

        // 创建
        return new(devicArgs)
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