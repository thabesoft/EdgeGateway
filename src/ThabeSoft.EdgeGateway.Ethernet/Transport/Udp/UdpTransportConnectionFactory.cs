using Microsoft.Extensions.Logging;

namespace ThabeSoft.EdgeGateway.Transport.Udp;


internal sealed class UdpTransportConnectionFactory(ILoggerFactory  loggerFactory) : ITransportConnectionFactory
{
    public bool CanCreate(ITransportDescriptor descriptor)
    {
        return descriptor is IpTransportDescriptor ip && ip.TransportType == IpTransportType.Udp;
    }

    public ITransportConnection Create(ITransportDescriptor descriptor)
    {
        if (descriptor is not IpTransportDescriptor ip || ip.TransportType != IpTransportType.Udp)
            throw new InvalidOperationException();

        return new UdpTransportConnection(ip, loggerFactory.CreateLogger<UdpTransportConnection>());
    }
}
