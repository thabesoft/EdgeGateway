using EdgeGateway.Channels;
using EdgeGateway.Devices;

namespace EdgeGateway.Ports;

public interface IPortFactory
{
    IPort Create(IChannelDescriptor channelDescriptor);
}

public sealed class PortFactory(
        IEnumerable<IChannelFactory> channelFactories,
        IEnumerable<IDeviceFactory> deviceFactories
    ) : IPortFactory
{
    public IPort Create(IChannelDescriptor channelDescriptor)
    {
        var channelFactory = channelFactories.FirstOrDefault(x => x.CanCreate(channelDescriptor)) ??
            throw new InvalidOperationException();

        return Port.Create(channelFactory, channelDescriptor, deviceFactories);
    }
}
