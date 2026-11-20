using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Simulation;


public sealed class SimulatorCreationArgs
{
    public LifecycleStateMachineDescriptor SimulatorStateMachine { get; init => field = value.Required(); } = LifecycleBuilder.RetryableStateMachine();

    public LifecycleBatchExecutorDescriptor PortBatchExecutor { get; init => field = value.Required(); } = LifecycleBuilder.BatchExecutor();
    public LifecycleStateMachineDescriptor PortStateMachine { get; init => field = value.Required(); } = LifecycleBuilder.RetryableStateMachine();

    public LifecycleBatchExecutorDescriptor DeviceBatchExecutor { get; init => field = value.Required(); } = LifecycleBuilder.BatchExecutor();
    public LifecycleStateMachineDescriptor DeviceStateMachine { get; init => field = value.Required(); } = LifecycleBuilder.RetryableStateMachine();
}
