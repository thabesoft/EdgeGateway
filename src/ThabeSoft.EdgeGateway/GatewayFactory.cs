using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Messages;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Primitives;
using ThabeSoft.PubSub;

namespace ThabeSoft.EdgeGateway;


internal sealed class GatewayFactory(
        ILifecycleStateMachineFactory lifecycleStateMachineFactory,
        IEventPublisher eventPublisher,
        PortManagerFactory portManagerFactory,
        PortFactory portFactory,
        ILoggerFactory loggerFactory
    )
{
    public Gateway Create(GatewayDescriptor descriptor)
    {
        var logger = loggerFactory.CreateLogger<Gateway>();

        // 状态机
        var gatewayStateMachine = lifecycleStateMachineFactory.Create(descriptor.StateMachine);
        var gatewayStateEventDisposable = gatewayStateMachine.StateChanged.Subscribe(x =>
            _ = eventPublisher.PublishAsync(new GatewayStateChangedMessage(x.Old, x.New)));


        var portManager = portManagerFactory.Create(new()
        {
            GatwayLifecycleState = gatewayStateMachine,
            PortBatchExecutorDescriptor = descriptor.PortBatchExecutor,
            PortCreator = port => portFactory.Create(new()
            {
                GatewayLifecycleState = gatewayStateMachine,
                Port = port
            })
        });

        var gatewayDependencies = new GatewayDependencies()
        {
            StateMachine = gatewayStateMachine,
            PortManager = portManager,

            Logger = logger,
            OnDispose = async () =>
            {
                await Safe.DisposeSafelyAsync(portManager, logger);
                await Safe.DisposeSafelyAsync(gatewayStateEventDisposable, logger);
                await Safe.DisposeSafelyAsync(gatewayStateMachine, logger);
            }
        };

        return new Gateway(gatewayDependencies);
    }
}