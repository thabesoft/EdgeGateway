using EdgeGateway.Devices;
using EdgeGateway.Ports;

namespace EdgeGateway;


public sealed record class GatewayOptions
{
    public required IReadOnlyList<IPortConfig> Ports { get; init; }
    public required IReadOnlyList<DeviceConfig> Devices { get; init; }
}
