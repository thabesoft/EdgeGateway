using ThabeSoft.EdgeGateway.Devices;

namespace ThabeSoft.EdgeGateway.Options;

public sealed record class DeviceOptions
{
    public required IDeviceDescriptor Descriptor { get; init; }

    internal DeviceOptions() { }
    public static DeviceOptionsBuilder Builder() => new();
}