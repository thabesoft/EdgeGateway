using EdgeGateway.Channel;
using EdgeGateway.Port;

namespace EdgeGateway.Config;

public sealed record class PortConfig
{
    public required IPortDescriptor Descriptor { get; init; }
    public required IReadOnlyList<DeviceConfig> Devices { get; init; }



    internal PortConfig() { }
    public static PortConfigBuilder Builder() => new();
}



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

        _descriptor = ChannelPortDescriptor.From(channelDescriptor);
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