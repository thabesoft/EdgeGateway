using EdgeGateway.Devices;
using EdgeGateway.Ports;

namespace EdgeGateway;


public record class GatewayOptions
{
    public required IReadOnlyList<IPortOptions> Ports { get; init; }
    public required IReadOnlyList<DeviceOptions> Devices { get; init; }
}