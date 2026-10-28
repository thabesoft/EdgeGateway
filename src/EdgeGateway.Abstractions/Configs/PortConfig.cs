using EdgeGateway.Port;

namespace EdgeGateway.Configs;


public sealed record class PortConfig
{
    public required IPortDescriptor Descriptor { get; init; }
    public required IReadOnlyList<DeviceConfig> Devices { get; init; }



    internal PortConfig() { }
    public static PortConfigBuilder Builder() => new();
}