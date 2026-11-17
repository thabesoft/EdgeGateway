using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Ports;


public sealed record class PortDescriptor
{
    public PortId Id { get; init => field = value.Required(); } = PortId.Create();
    public required TransportDescriptor Transport { get; init => field = value.Required(); }
}