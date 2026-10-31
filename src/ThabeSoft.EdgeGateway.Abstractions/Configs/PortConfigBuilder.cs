using EdgeGateway.Channel;
using EdgeGateway.Port;

namespace EdgeGateway.Configs;


public sealed class PortConfigBuilder
{
    private IPortDescriptor? _descriptor;
    private readonly List<DeviceConfig> _devices = [];


    public PortConfigBuilder SetDescriptor(IPortDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        _descriptor = descriptor;
        return this;
    }
    public PortConfigBuilder SetDescriptor(IChannelDescriptor channelDescriptor)
    {
        ArgumentNullException.ThrowIfNull(channelDescriptor);

        _descriptor = PortDescriptor.From(channelDescriptor);
        return this;
    }


    public PortConfigBuilder AddDevice(DeviceConfig config)
    {
        if (_devices.Contains(config))
            return this;

        _devices.Add(config);
        return this;
    }


    public PortConfig Build()
    {
        ArgumentNullException.ThrowIfNull(_descriptor);


        return new PortConfig()
        {
            Descriptor = _descriptor,
            Devices = _devices
        };
    }
}