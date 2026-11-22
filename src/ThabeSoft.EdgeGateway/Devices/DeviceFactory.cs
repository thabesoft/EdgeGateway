using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Messages;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Primitives;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.Protocols.Accessor;
using ThabeSoft.PubSub;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class DeviceFactory(
    ILifecycleStateMachineFactory lifecycleStateMachineFactory,
    SubscriptionManagerFactory subscriptionManagerFactory,
    IEventPublisher eventPublisher,
    SubscriptionFactory subscriptionFactory,
    ILoggerFactory loggerFactory)
{
    public Device Create(DeviceCreationArgs args)
    {
        var logger = loggerFactory.CreateLogger<Device>();

        // 状态机
        var deviceStateMachine = lifecycleStateMachineFactory.Create(args.Device.StateMachine);
        // 状态改变事件
        var deviceStateEventDisposable = deviceStateMachine.StateChanged.Subscribe(x =>
            _ = eventPublisher.PublishAsync(new DeviceStateChangedMessage(args.Port.Id, args.Device.Id, x.Old, x.New)));

        // 订阅管理器
        var subscriptionManager = subscriptionManagerFactory.Create(new()
        {
            Port = args.Port,
            ProtocolAccessor = args.ProtocolAccessor,

            Device = args.Device,
            DeviceState = deviceStateMachine,

            SubscriptionFactory = subscriptionFactory
        });

        // 设备依赖
        var deviceDependencies = new DeviceDependencies()
        {
            Port = args.Port,
            PortState = args.PortState,

            Descriptor = args.Device,
            StateMachine = deviceStateMachine,

            SubscriptionManager = subscriptionManager,

            Logger = logger,
            OnDispose = async () =>
            {
                await Safe.TryDisposeAsync(subscriptionManager, logger);
                await Safe.TryDisposeAsync(deviceStateEventDisposable, logger);
                await Safe.TryDisposeAsync(deviceStateMachine, logger);
            }
        };

        // 构建
        return new(deviceDependencies);
    }
}


internal readonly struct DeviceCreationArgs
{
    // Port
    public required PortDescriptor Port { get; init => field = value.Required(); }
    public required ILifecycleStateable PortState { get; init; }
    public required IProtocolAccessor ProtocolAccessor { get; init => field = value.Required(); }

    // Device
    public required DeviceDescriptor Device { get; init => field = value.Required(); }
}