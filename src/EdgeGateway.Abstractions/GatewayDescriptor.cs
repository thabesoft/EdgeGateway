using EdgeGateway.Devices;

namespace EdgeGateway;


public record class GatewayDescriptor
{
    public required IReadOnlyList<IPortOptions> Ports { get; init; }
    public required IReadOnlyList<DeviceDescriptor> Devices { get; init; }
}