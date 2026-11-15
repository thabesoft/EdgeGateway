using ThabeSoft.EdgeGateway.Compatibility;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


public sealed record class PortDescriptor
{
    public PortId Id { get; init => field = value.Required(); } = PortId.Create();
    public required TransportDescriptor Transport { get; init => field = value.Required(); }
}