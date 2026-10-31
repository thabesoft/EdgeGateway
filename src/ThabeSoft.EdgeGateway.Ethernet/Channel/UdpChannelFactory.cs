using EdgeGateway.Channel;

namespace EdgeGateway.Ethernet.Channel;

internal sealed class UdpChannelFactory : IChannelFactory
{
    public bool CanCreate(IChannelDescriptor descriptor)
    {
        return descriptor is IpChannelDescriptor ip && ip.Connection == IpConnectionType.Udp;
    }

    public IChannel Create(IChannelDescriptor descriptor)
    {
        if (descriptor is not IpChannelDescriptor ip || ip.Connection != IpConnectionType.Udp)
            throw new InvalidOperationException();

        return new UdpChannel(ip);
    }
}
