using EdgeGateway.Ports;
using EdgeGateway.Ports.Ethernet;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace EdgeGateway.Ethernet;


internal sealed class TcpPortFactory(ILoggerFactory loggerFactory) : IPortFactory
{
    public bool CanHandle(IPortOptions config)
    {
        return config is EthernetPortOptions && config.Address.PortType == PortType.Tcp;
    }

    public Result<IPort> Create(IPortOptions config)
        => new TcpPortAdapter((EthernetPortOptions)config, loggerFactory.CreateLogger<TcpPortAdapter>());
}