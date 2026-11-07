namespace ThabeSoft.EdgeGateway.Transport;

internal sealed class UdpTransportConnectionFactory : ITransportConnectionFactory
{
    public bool CanCreate(ITransportDescriptor descriptor)
    {
        return descriptor is IpTransportDescriptor ip && ip.TransportType == IpTransportType.Udp;
    }

    public ITransportConnection Create(ITransportDescriptor descriptor)
    {
        if (descriptor is not IpTransportDescriptor ip || ip.TransportType != IpTransportType.Udp)
            throw new InvalidOperationException();

        return new UdpTransportConnection(ip);
    }
}
