using EdgeGateway.Ports;
using EdgeGateway.Ports.Ethernet;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace EdgeGateway.Ethernet;


internal sealed class TcpPortFactory(ILoggerFactory loggerFactory) : IPortFactory
{
    public bool CanCreate(IPortOptions config)
    {
        return config is EthernetPortOptions && config.Address.PortType == TransportProtocol.Tcp;
    }

    public Result<IPort> Create(IPortOptions config)
        => new TcpPortAdapter((EthernetPortOptions)config, loggerFactory.CreateLogger<TcpPortAdapter>());
}