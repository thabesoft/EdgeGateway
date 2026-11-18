using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway;

internal readonly struct SimulatorArgs
{
    public required LifecycleStateMachineDescriptor StateMachine { get; init => field = value.Required(); }

    public required SimulationPortFactory PortFactoryFactory { get; init => field = value.Required(); }
    public required LifecycleBatchExecutorDescriptor PortBatchExecutor { get; init => field = value.Required(); }


    public required ILifecycleBatchExecutorFactory LifecycleBatchExecutorFactory { get; init => field = value.Required(); }
    public required ILifecycleStateMachineFactory LifecycleStateMachineFactory { get; init => field = value.Required(); }
    public required ILogger Logger { get; init => field = value.Required(); }
}