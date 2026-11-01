using ThabeSoft.EdgeGateway.Devices;

namespace ThabeSoft.EdgeGateway.Configs;

public sealed record class DeviceConfig
{
    public required IDeviceDescriptor Descriptor { get; init; }


    internal DeviceConfig() { }
    public static DeviceConfigBuilder Builder() => new();
}