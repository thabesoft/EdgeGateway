
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Transports;

public interface ITransportConnectionFactory
{
    ITransportConnection Create(TransportCreationContext context);
}

public readonly struct TransportCreationContext
{
    public required TransportDescriptor Transport { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor StateMachine { get; init => field = value.Required(); }
}