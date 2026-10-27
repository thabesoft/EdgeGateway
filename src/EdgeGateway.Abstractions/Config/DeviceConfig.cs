using EdgeGateway.Device;

namespace EdgeGateway.Config;

public sealed record class DeviceConfig
{
    public required IDeviceDescriptor Descriptor { get; init; }


    internal DeviceConfig() { }
    public static DeviceConfigBuilder Builder() => new();
}


public sealed record class DeviceConfigBuilder
{
    private IDeviceDescriptor? _descriptor;


    public DeviceConfigBuilder SetDescriptor(IDeviceDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        _descriptor = descriptor;
        return this;
    }

    public DeviceConfig Build()
    {
        ArgumentNullException.ThrowIfNull(_descriptor);


        return new()
        {
            Descriptor = _descriptor
        };
    }
}
