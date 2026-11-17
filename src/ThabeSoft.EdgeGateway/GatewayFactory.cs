using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway;


internal sealed class GatewayFactory(
        ILifecycleStateMachineFactory lifecycleStateMachineFactory,
        IEventPublisher eventPublisher,
        PortManagerFactory portManagerFactory,
        PortFactory portFactory,
        ILoggerFactory loggerFactory
    )
{
    public Gateway Create(GatewayCreationArgs context)
    {
        var logger = loggerFactory.CreateLogger<Gateway>();

        // 状态机
        var gatewayStateMachine = lifecycleStateMachineFactory.Create(context.GatewayStateMachine);
        var gatewayStateEventDisposable = gatewayStateMachine.StateChanged.Subscribe(x =>
            _ = eventPublisher.PublishAsync(new GatewayStateChangedEvent(x.Old, x.New)));


        var portManager = portManagerFactory.Create(new()
        {
            GatwayLifecycleState = gatewayStateMachine,
            PortBatchExecutorDescriptor = context.PortBatchExecutor,
            PortCreator = port => portFactory.Create(new()
            {
                GatewayLifecycleState = gatewayStateMachine,

                Port = port,
                PortStateMachine = context.PortStateMachine,
                TransportStateMachine = context.TransportStateMachine,

                DeviceStateMachine = context.DeviceStateMachine,
                DeviceBatchExecutor = context.DeviceBatchExecutorDescriptor,

                SubscriptionStateMachine = context.SubscriptionStateMachine,
                SubscriptionBatchExecutor = context.SubscriptionBatchExecutorDescriptor,
            })
        });

        // 网关依赖
        var gatewaytDependencies = new GatewayArgs()
        {
            GatwayStateMachine = gatewayStateMachine,
            PortManager = portManager,
            Logger = logger
        };


        return new Gateway(gatewaytDependencies)
        {
            DisposedCallback = async () =>
            {
                await portManager.TryDisposeAsync();
                gatewayStateEventDisposable.TryDispose();
                await gatewayStateMachine.TryDisposeAsync();
            }
        };
    }
}