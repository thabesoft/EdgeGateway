using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Devices;


public sealed record class DeviceDescriptor
{
    public DeviceId Id { get; init => field = value.Required(); } = DeviceId.Create();
    public required ProtocolDescriptor Protocol { get; init => field = value.Required(); }
}