using ThabeSoft.EdgeGateway.Channel;
using ThabeSoft.EdgeGateway.Device;

namespace ThabeSoft.EdgeGateway.Port;


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
        var channelFactory = channelFactories.FirstOrDefault(x => x.CanCreate(descriptor.ChannelDescriptor)) ??
            throw new InvalidOperationException($"没有能创建该端口的工厂: {descriptor.GetType()}");

        return DefaultPort.Create(channelFactory, descriptor.ChannelDescriptor, deviceFactories);
    }
}
