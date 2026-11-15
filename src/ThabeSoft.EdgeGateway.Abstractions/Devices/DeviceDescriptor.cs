using ThabeSoft.EdgeGateway.Compatibility;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Devices;


public sealed record class DeviceDescriptor
{
    public DeviceId Id { get; init => field = value.Required(); } = DeviceId.Create();
    public required ProtocolDescriptor Protocol { get; init => field = value.Required(); }
}