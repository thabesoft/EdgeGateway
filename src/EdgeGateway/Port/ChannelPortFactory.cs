using EdgeGateway.Channel;
using EdgeGateway.Device;

namespace EdgeGateway.Port;


internal sealed class ChannelPortFactory(
        IEnumerable<IChannelFactory> channelFactories,
        IEnumerable<IDeviceFactory> deviceFactories
    ) : IPortFactory
{
    public bool CanCreate(IPortDescriptor descriptor)
    {
        return descriptor is ChannelPortDescriptor;
    }

    public IPort Create(IPortDescriptor descriptor)
    {
        if (descriptor is not ChannelPortDescriptor channelPort)
            throw new InvalidOperationException();

        var channelFactory = channelFactories.FirstOrDefault(x => x.CanCreate(channelPort.ChannelDescriptor)) ??
            throw new InvalidOperationException();

        return ChannelPort.Create(channelFactory, channelPort.ChannelDescriptor, deviceFactories);
    }
}
