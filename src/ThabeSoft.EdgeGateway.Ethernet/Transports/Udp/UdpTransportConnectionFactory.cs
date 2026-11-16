using Microsoft.Extensions.Logging;

namespace ThabeSoft.EdgeGateway.Transports.Udp;


internal sealed class UdpTransportConnectionFactory(ILoggerFactory  loggerFactory) : ITransportConnectionFactory
{
    public bool CanCreate(TransportDescriptor descriptor)
    {
        return descriptor is IpTransportDescriptor ip && ip.ProtocolType == IpProtocolType.Udp;
    }

    public ITransportConnection Create(TransportDescriptor descriptor)
    {
        if (descriptor is not IpTransportDescriptor ip || ip.ProtocolType != IpProtocolType.Udp)
            throw new InvalidOperationException();

        return new UdpTransportConnection(ip, loggerFactory.CreateLogger<UdpTransportConnection>());
    }
}
