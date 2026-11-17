using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway;


public sealed record GatewayDescriptor
{
    public required LifecycleStateMachineDescriptor GatewayStateMachine { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor PortStateMachine { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor DeviceStateMachine { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor SubscriptionStateMachine { get; init => field = value.Required(); }
}