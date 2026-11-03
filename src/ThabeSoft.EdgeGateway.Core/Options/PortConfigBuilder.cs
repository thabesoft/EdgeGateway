using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.Options;


public sealed class PortConfigBuilder : IPortOptionsBuilder
{
    private IPortDescriptor? _descriptor;
    private readonly List<DeviceOptions> _devices = [];


    public IPortOptionsBuilder SetDescriptor(IPortDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        _descriptor = descriptor;
        return this;
    }
    public IPortOptionsBuilder AddDevice(Action<IDeviceOptionsBuilder> buildAction)
    {
        var builder = new DeviceOptionsBuilder();
        buildAction.Invoke(builder);
        var deviceOptions = builder.Build();

        if (_devices.Contains(deviceOptions))
            return this;

        _devices.Add(deviceOptions);
        return this;
    }


    public PortOptions Build()
    {
        if (_descriptor is null)
            throw new ArgumentNullException(nameof(_descriptor), "未初始化端口信息");

        return new PortOptions()
        {
            Descriptor = _descriptor,
            Devices = _devices
        };
    }
}