namespace ThabeSoft.EdgeGateway.Channels;

internal sealed class UdpChannelFactory : IChannelFactory
{
    public bool CanCreate(IChannelDescriptor descriptor)
    {
        return descriptor is IpChannelDescriptor ip && ip.ConnectionType == IpConnectionType.Udp;
    }

    public IChannel Create(IChannelDescriptor descriptor)
    {
        if (descriptor is not IpChannelDescriptor ip || ip.ConnectionType != IpConnectionType.Udp)
            throw new InvalidOperationException();

        return new UdpChannel(ip);
    }
}
