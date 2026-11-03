using ThabeSoft.EdgeGateway.Devices;

namespace ThabeSoft.EdgeGateway.Options;


public sealed record class DeviceOptionsBuilder : IDeviceOptionsBuilder
{
    private IDeviceDescriptor? _descriptor;


    public IDeviceOptionsBuilder SetDescriptor(IDeviceDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        _descriptor = descriptor;
        return this;
    }

    public DeviceOptions Build()
    {
        ArgumentNullException.ThrowIfNull(_descriptor);

        return new()
        {
            Descriptor = _descriptor
        };
    }
}