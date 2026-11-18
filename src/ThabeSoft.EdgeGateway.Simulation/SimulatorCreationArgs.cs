using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway;


public sealed class SimulatorCreationArgs
{
    public required LifecycleStateMachineDescriptor StateMachine { get; init => field = value.Required(); }
    public required LifecycleBatchExecutorDescriptor PortBatchExecutor { get; init => field = value.Required(); }
}
