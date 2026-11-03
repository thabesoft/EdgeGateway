using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.Options;

public sealed record class PortOptions
{
    public required IPortDescriptor Descriptor { get; init; }
    public required IReadOnlyList<DeviceOptions> Devices { get; init; }



    internal PortOptions() { }
    public static PortConfigBuilder Builder() => new();
}
