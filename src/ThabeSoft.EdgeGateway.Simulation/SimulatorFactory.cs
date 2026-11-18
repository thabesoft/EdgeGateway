using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.Simulation;

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
            SimulatorStateMachine = args.SimulatorStateMachine,

            PortFactory = portFactory,
            PortBatchExecutor = args.PortBatchExecutor,
            PortStateMachine = args.PortStateMachine,

            DeviceStateMachine = args.DeviceStateMachine,
            DeviceBatchExecutor = args.DeviceBatchExecutor,

            LifecycleBatchExecutorFactory = lifecycleBatchExecutorFactory,
            LifecycleStateMachineFactory = lifecycleStateMachineFactory,
            Logger = loggerFactory.CreateLogger<Simulator>()
        };

        return new(simulatorArgs);
    }
}
