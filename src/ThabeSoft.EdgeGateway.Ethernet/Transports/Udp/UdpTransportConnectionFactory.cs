using Microsoft.Extensions.Logging;

namespace ThabeSoft.EdgeGateway.Transports.Udp;


internal sealed class UdpTransportConnectionFactory(ILoggerFactory  loggerFactory) : ITransportConnectionResolver
{
    public bool CanResolve(TransportDescriptor descriptor)
    {
        return descriptor is IpTransportDescriptor ip && ip.ProtocolType == IpProtocolType.Udp;
    }

    public ITransportConnection Resolve(TransportDescriptor descriptor)
    {
        if (descriptor is not IpTransportDescriptor ip || ip.ProtocolType != IpProtocolType.Udp)
            throw new InvalidOperationException();

        return new UdpTransportConnection(ip, loggerFactory.CreateLogger<UdpTransportConnection>());
    }
}
