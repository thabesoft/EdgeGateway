using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Transports;


public abstract record class TransportDescriptor
{
    public abstract ConnectionEndpoint Endpoint { get; }
    public RetryDescriptor Retry { get; init => field = value.Required(); } = RetryBuilder.Default;
    public LifecycleStateMachineDescriptor StateMachine { get; init => field = value.Required(); } = LifecycleBuilder.StrictStateMachine();
}