using EdgeGateway.Channel;
using EdgeGateway.Device;

namespace EdgeGateway.Port;


internal sealed class PortFactory(
        IEnumerable<IChannelFactory> channelFactories,
        IEnumerable<IDeviceFactory> deviceFactories
    ) : IPortFactory
{
    public bool CanCreate(IPortDescriptor descriptor)
    {
        return descriptor is PortDescriptor;
    }

    public IPort Create(IPortDescriptor descriptor)
    {
        if (descriptor is not PortDescriptor channelPort)
            throw new InvalidOperationException();

        var channelFactory = channelFactories.FirstOrDefault(x => x.CanCreate(channelPort.ChannelDescriptor)) ??
            throw new InvalidOperationException();

        return DefaultPort.Create(channelFactory, channelPort.ChannelDescriptor, deviceFactories);
    }
}
