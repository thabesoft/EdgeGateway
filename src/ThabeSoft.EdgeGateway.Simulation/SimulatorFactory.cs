using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway;

internal sealed class SimulatorFactory(
        SimulationPortFactory portFactory,
        ILifecycleBatchExecutorFactory lifecycleBatchExecutorFactory,
        ILifecycleStateMachineFactory lifecycleStateMachineFactory,
        ILoggerFactory loggerFactory
    )
{
    public Simulator Create(in SimulatorCreationArgs args)
    {
        var simulatorArgs = new SimulatorArgs()
        {
            StateMachine = args.StateMachine,
            PortFactoryFactory = portFactory,
            PortBatchExecutor = args.PortBatchExecutor,
            LifecycleBatchExecutorFactory = lifecycleBatchExecutorFactory,
            LifecycleStateMachineFactory = lifecycleStateMachineFactory,
            Logger = loggerFactory.CreateLogger<Simulator>()
        };

        return new(simulatorArgs);
    }
}
