using ThabeSoft.EdgeGateway.Devices;

namespace ThabeSoft.EdgeGateway.Configs;

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
