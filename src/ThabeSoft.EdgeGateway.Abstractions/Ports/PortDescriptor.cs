using ThabeSoft.EdgeGateway.Primitives.Guard;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Ports;


public sealed record class PortDescriptor
{
    public PortId Id { get; init => field = value.Required(); } = PortId.Create();
    public required TransportDescriptor Transport { get; init => field = value.Required(); }
}