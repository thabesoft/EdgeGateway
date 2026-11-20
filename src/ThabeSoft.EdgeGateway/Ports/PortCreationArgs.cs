using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Ports;

internal readonly struct PortCreationArgs
{
    // Gateway
    public required ILifecycleStateable GatewayLifecycleState { get; init => field = value.Required(); }


    // Port
    public required PortDescriptor Port { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor PortStateMachine { get; init; }
    public required LifecycleStateMachineDescriptor TransportStateMachine { get; init => field = value.Required(); }

    // Device
    public required LifecycleStateMachineDescriptor DeviceStateMachine { get; init => field = value.Required(); }
    public required LifecycleBatchExecutorDescriptor DeviceBatchExecutor { get; init => field = value.Required(); }


    // Subscription
    public required LifecycleStateMachineDescriptor SubscriptionStateMachine { get; init => field = value.Required(); }
    public required LifecycleBatchExecutorDescriptor SubscriptionBatchExecutor { get; init => field = value.Required(); }
}