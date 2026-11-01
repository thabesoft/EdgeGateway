namespace ThabeSoft.EdgeGateway.Channels;

internal sealed class TcpChannelFactory : IChannelFactory
{
    public bool CanCreate(IChannelDescriptor descriptor)
    {
        return descriptor is IpChannelDescriptor ip && ip.Connection == IpConnectionType.Tcp;
    }

    public IChannel Create(IChannelDescriptor descriptor)
    {
        if (descriptor is not IpChannelDescriptor ip || ip.Connection != IpConnectionType.Tcp)
            throw new InvalidOperationException();

        return new TcpChannel(ip);
    }
}
