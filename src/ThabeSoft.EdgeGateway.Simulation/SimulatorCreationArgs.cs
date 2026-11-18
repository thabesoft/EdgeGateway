using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Simulation;


public sealed class SimulatorCreationArgs
{
    public required LifecycleStateMachineDescriptor SimulatorStateMachine { get; init => field = value.Required(); } = LifecycleBuilder.RetryableStateMachine();

    public required LifecycleBatchExecutorDescriptor PortBatchExecutor { get; init => field = value.Required(); } = LifecycleBuilder.BatchExecutor();
    public required LifecycleStateMachineDescriptor PortStateMachine { get; init => field = value.Required(); } = LifecycleBuilder.RetryableStateMachine();

    public required LifecycleBatchExecutorDescriptor DeviceBatchExecutor { get; init => field = value.Required(); } = LifecycleBuilder.BatchExecutor();
    public required LifecycleStateMachineDescriptor DeviceStateMachine { get; init => field = value.Required(); } = LifecycleBuilder.RetryableStateMachine();
}
