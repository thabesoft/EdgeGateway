using ThabeSoft.EdgeGateway.Device;
using ThabeSoft.EdgeGateway.Polling.Triggers;
using ThabeSoft.EdgeGateway.Protocol;
using ThabeSoft.EdgeGateway.Tag;

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
    public DeviceConfigBuilder Ethernet(IProtocolDescriptor protocolDescriptor) =>
        SetDescriptor(DeviceDescriptor.Ethernet(protocolDescriptor));

    public DeviceConfigBuilder Serial(IProtocolDescriptor protocolDescriptor) =>
        SetDescriptor(DeviceDescriptor.Serial(protocolDescriptor));


    public DeviceConfig Build()
    {
        ArgumentNullException.ThrowIfNull(_descriptor);

        return new()
        {
            Descriptor = _descriptor
        };
    }
}
