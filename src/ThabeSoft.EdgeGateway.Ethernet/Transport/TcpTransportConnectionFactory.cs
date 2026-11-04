namespace ThabeSoft.EdgeGateway.Transport;


internal sealed class TcpTransportConnectionFactory : ITransportFactory
{
    public bool CanCreate(ITransportDescriptor descriptor)
    {
        return descriptor is IpTransportDescriptor ip && ip.TransportType == IpTransportType.Tcp;
    }

    public ITransportConnection Create(ITransportDescriptor descriptor)
    {
        if (descriptor is not IpTransportDescriptor ip || ip.TransportType != IpTransportType.Tcp)
            throw new InvalidOperationException();

        return new TcpTransportConnection(ip);
    }
}
