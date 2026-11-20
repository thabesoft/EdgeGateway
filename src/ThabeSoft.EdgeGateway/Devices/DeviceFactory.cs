using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.Primitives;
using ThabeSoft.Protocols;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class DeviceFactory(
    ILifecycleStateMachineFactory lifecycleStateMachineFactory,
    SubscriptionManagerFactory subscriptionManagerFactory,
    IEventPublisher eventPublisher,
    SubscriptionFactory subscriptionFactory,
    ILoggerFactory loggerFactory)
{
    public Device Create(in DeviceCreationArgs args)
    {
        var logger = loggerFactory.CreateLogger<Device>();

        var portId = args.Port.Id;
        var deviceId = args.Device.Id;

        // 状态机
        var deviceStateMachine = lifecycleStateMachineFactory.Create(args.DeviceStateMachineDescriptor);
        var deviceStateEventDisposable = deviceStateMachine.StateChanged.Subscribe(x =>
            _ = eventPublisher.PublishAsync(new DeviceStateChangedEvent(portId, deviceId, x.Old, x.New)));

        // 订阅创建器
        var subscriptionCreator = new SubscriptionCreator(new()
        {
            Port = args.Port,
            ProtocolAccessor = args.ProtocolAccessor,

            Device = args.Device,
            DeviceState = deviceStateMachine,

            SubscriptionStateMachineDescriptor = args.SubscriptionStateMachineDescriptor,
            SubscriptionFactory = subscriptionFactory,
        });

        // 订阅管理器
        var subscriptionManager = subscriptionManagerFactory.Create(new()
        {
            Device = args.Device,
            Port = args.Port,
            DeviceState = deviceStateMachine,
            SubscriptionCreator = subscriptionCreator,
            SubscriptionBatchExecutor = args.SubscriptionBatchExecutorDescriptor
        });

        // 构建
        return new
        (
            id: args.Device.Id,
            protocolType: args.Device.Protocol.Type,
            stateMachine: deviceStateMachine,
            subscriptionManager: subscriptionManager,
            portState: args.PortState,
            logger: logger,
            onDispose: async () =>
            {
                await Safe.TryDisposeAsync(subscriptionManager, logger);
                await Safe.TryDisposeAsync(deviceStateEventDisposable, logger);
                await Safe.TryDisposeAsync(deviceStateMachine, logger);
            }
        );
    }


    private sealed class SubscriptionCreator(SubscriptionCreatorDependencies dependencies) : ISubscriptionCreator
    {
        public Subscription<T> Create<T>(SubscriptionDescriptor<T> descriptor) where T : notnull
        {
            return dependencies.SubscriptionFactory.Create<T>(new()
            {
                Port = dependencies.Port,
                ProtocolAccessor = dependencies.ProtocolAccessor,
                Device = dependencies.Device,
                DeviceState = dependencies.DeviceState,
                Subscription = descriptor,
                SubscriptioStateMachineDescriptor = dependencies.SubscriptionStateMachineDescriptor
            });
        }
    }

    private readonly struct SubscriptionCreatorDependencies
    {
        // Port
        public required PortDescriptor Port { get; init => field = value.Required(); }
        public required IProtocolAccessor ProtocolAccessor { get; init => field = value.Required(); }

        // Device
        public required DeviceDescriptor Device { get; init => field = value.Required(); }
        public required ILifecycleStateable DeviceState { get; init => field = value.Required(); }

        // Subscription
        public required LifecycleStateMachineDescriptor SubscriptionStateMachineDescriptor { get; init => field = value.Required(); }
        public required SubscriptionFactory SubscriptionFactory { get; init => field = value.Required(); }
    }
}