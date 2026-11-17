using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway;


public sealed class GatewayCreationArgs
{
    // Gateway
    public required LifecycleStateMachineDescriptor GatewayStateMachine { get; init => field = value.Required(); }

    // Port
    public required LifecycleStateMachineDescriptor PortStateMachine { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor TransportStateMachine { get; init => field = value.Required(); }
    public LifecycleBatchExecutorDescriptor PortBatchExecutor { get; init => field = value.Required(); } = new LifecycleBatchExecutorDescriptor();


    // Device
    public required LifecycleStateMachineDescriptor DeviceStateMachine { get; init => field = value.Required(); }
    public LifecycleBatchExecutorDescriptor DeviceBatchExecutor { get; init => field = value.Required(); } = new LifecycleBatchExecutorDescriptor();


    // Subscription
    public required LifecycleStateMachineDescriptor SubscriptionStateMachine { get; init => field = value.Required(); }
    public LifecycleBatchExecutorDescriptor SubscriptionBatchExecutor { get; init => field = value.Required(); } = new LifecycleBatchExecutorDescriptor();
}