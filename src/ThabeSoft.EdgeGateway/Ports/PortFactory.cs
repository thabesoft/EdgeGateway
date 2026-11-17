using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class PortFactory(
        ILifecycleStateMachineFactory lifecycleStateMachineFactory,
        IEventPublisher eventPublisher,
        DeviceManagerFactory deviceManagerFactory,
        ITransportConnectionFactory transportConnectionFactory,
        ProtocolAccessorFactory protocolAccessorResolver,
        DeviceFactory deviceFactory,
        ILoggerFactory loggerFactory
    )
{
    public Port Create(in PortCreationArgs context)
    {
        var logger = loggerFactory.CreateLogger<Port>();

        var port = context.Port;
        var deviceStateMachine = context.DeviceStateMachine;
        var subscriptionBatchExecutor = context.SubscriptionBatchExecutor;
        var subscriptionStateMachine = context.SubscriptionStateMachine;


        // 状态机
        var portStateMachine = lifecycleStateMachineFactory.Create(context.PortStateMachine);
        var deviceStateEventDisposable = portStateMachine.StateChanged.Subscribe(x =>
            _ = eventPublisher.PublishAsync(new PortStateChangedEvent(port.Id, x.Old, x.New)));

        // 传输器
        var transport = transportConnectionFactory.Create(new()
        {
            Transport = context.Port.Transport,
            StateMachine = context.TransportStateMachine
        });
        // 协议访问器
        var protocolAccessors = new ConcurrentDictionary<ProtocolDescriptor, IProtocolAccessor>();

        // 设备管理器
        var deviceManager = deviceManagerFactory.Create(new()
        {
            Port = context.Port,
            PortLifecycleState = portStateMachine,

            DeviceBatchExecutorDescriptor = context.DeviceBatchExecutor,
            DevicCreator = device => deviceFactory.Create(new()
            {
                Port = port,
                ProtocolAccessor = protocolAccessors.GetOrAdd(device.Protocol, protocol => protocolAccessorResolver.Create(protocol, transport)),
                PortState = portStateMachine,

                Device = device,
                DeviceStateMachineDescriptor = deviceStateMachine,

                SubscriptionBatchExecutorDescriptor = subscriptionBatchExecutor,
                SubscriptionStateMachineDescriptor = subscriptionStateMachine
            })
        });

        // 端口依赖
        var portDependencies = new PortArgs()
        {
            GatewayLifecycleState = context.GatewayLifecycleState,

            Port = context.Port,
            PortStateMachine = portStateMachine,

            DeviceManager = deviceManager,
            TransportConnectionLifeCycle = transport,

            Logger = logger
        };

        return new Port(portDependencies)
        {
            DisposedCallback = async () =>
            {
                await deviceManager.TryDisposeAsync();
                protocolAccessors.Clear();
                await transport.TryDisposeAsync();
                deviceStateEventDisposable.TryDispose();
                await portStateMachine.TryDisposeAsync();
            }
        };
    }
}