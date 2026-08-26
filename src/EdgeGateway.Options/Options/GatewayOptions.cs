namespace EdgeGateway.Options;


public record class GatewayOptions
{
    public required IReadOnlyList<IPortOptions> Ports { get; init; }
    public required IReadOnlyList<DeviceOptions> Devices { get; init; }
}