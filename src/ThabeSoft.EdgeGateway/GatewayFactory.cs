using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Primitives;

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
                DeviceBatchExecutor = context.DeviceBatchExecutor,

                SubscriptionStateMachine = context.SubscriptionStateMachine,
                SubscriptionBatchExecutor = context.SubscriptionBatchExecutor,
            })
        });


        return new Gateway
        (
            _stateMachine: gatewayStateMachine,
            _portManager: portManager,
            _logger: logger,
            onDispose: async () =>
            {
                await Safe.DisposeSafelyAsync(portManager, logger);
                await Safe.DisposeSafelyAsync(gatewayStateEventDisposable, logger);
                await Safe.DisposeSafelyAsync(gatewayStateMachine, logger);
            }
        );
    }
}