using ThabeSoft.EdgeGateway.Port;

namespace ThabeSoft.EdgeGateway.Configs;


public sealed record class PortConfig
{
    public required IPortDescriptor Descriptor { get; init; }
    public required IReadOnlyList<DeviceConfig> Devices { get; init; }



    internal PortConfig() { }
    public static PortConfigBuilder Builder() => new();
}