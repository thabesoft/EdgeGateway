using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Messages;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Primitives;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.Protocols;
using ThabeSoft.PubSub;
using ThabeSoft.Transports;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class PortFactory(
        ILifecycleStateMachineFactory lifecycleStateMachineFactory,
        IEventPublisher eventPublisher,
        DeviceManagerFactory deviceManagerFactory,
        ITransportConnectionFactory transportConnectionFactory,
        IProtocolAccessorFactory protocolAccessorResolver,
        DeviceFactory deviceFactory,
        ILoggerFactory loggerFactory
    )
{
    public Port Create(PortCreationArgs args)
    {
        var logger = loggerFactory.CreateLogger<Port>();

        // 状态机
        var portStateMachine = lifecycleStateMachineFactory.Create(args.Port.StateMachine);
        // 状态变更事件
        var deviceStateEventDisposable = portStateMachine.StateChanged.Subscribe(x =>
            _ = eventPublisher.PublishAsync(new PortStateChangedMessage(args.Port.Id, x.Old, x.New)));

        // 传输器
        var transport = transportConnectionFactory.Create(args.Port.Transport);
        // 协议访问器
        var protocolAccessors = new ConcurrentDictionary<ProtocolDescriptor, IProtocolAccessor>();

        // 设备管理器
        var deviceManager = deviceManagerFactory.Create(new()
        {
            Port = args.Port,
            PortState = portStateMachine,
            ProtocolAccessorProvider = protocol => protocolAccessors.GetOrAdd(protocol, x => protocolAccessorResolver.Create(x, transport)),
            DevicCreator = device => deviceFactory.Create(new()
            {
                Port = args.Port,
                PortState = portStateMachine,
                ProtocolAccessor = protocolAccessors.GetOrAdd(device.Protocol, protocol => protocolAccessorResolver.Create(protocol, transport)),

                Device = device,
            })
        });

        // 端口依赖
        var portDependencies = new PortDependencies()
        {
            GatewayState = args.GatewayLifecycleState,

            Descriptor = args.Port,
            StateMachine = portStateMachine,
            TransportConnectionLifecycle = transport,

            DeviceManager = deviceManager,

            Logger = logger,
            OnDispose = async () =>
            {
                await Safe.DisposeSafelyAsync(deviceManager, logger);
                protocolAccessors.Clear();
                await Safe.DisposeSafelyAsync(transport, logger);
                await Safe.DisposeSafelyAsync(deviceStateEventDisposable, logger);
                await Safe.DisposeSafelyAsync(portStateMachine, logger);
            }
        };

        return new Port(portDependencies);
    }
}

internal sealed class PortCreationArgs
{
    // Gateway
    public required ILifecycleStateable GatewayLifecycleState { get; init => field = value.Required(); }

    // Port
    public required PortDescriptor Port { get; init => field = value.Required(); }
}