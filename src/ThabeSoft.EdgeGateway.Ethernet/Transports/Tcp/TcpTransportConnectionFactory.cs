using Microsoft.Extensions.Logging;

namespace ThabeSoft.EdgeGateway.Transports.Tcp;


internal sealed class TcpTransportConnectionFactory(ILoggerFactory loggerFactory) : ITransportConnectionFactory
{
    public bool CanCreate(TransportDescriptor descriptor)
    {
        return descriptor is IpTransportDescriptor ip && ip.ProtocolType == IpProtocolType.Tcp;
    }

    public ITransportConnection Create(TransportDescriptor descriptor)
    {
        if (descriptor is not IpTransportDescriptor ip || ip.ProtocolType != IpProtocolType.Tcp)
            throw new InvalidOperationException();

        return new TcpTransportConnection(ip, loggerFactory.CreateLogger<TcpTransportConnection>());
    }
}
