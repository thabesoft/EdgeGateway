using EdgeGateway.Options;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace EdgeGateway.Ports.Ethernet;


public sealed class EthernetPortFactory(ILoggerFactory loggerFactory) : IPortFactory
{
    public bool CanHandle(IPortOptions config)
    {
        return config is EthernetPortOptions;
    }

    public Result<IPort> Create(IPortOptions config)
    {
        if (config is not EthernetPortOptions ethernetPortConfig)
            return new Error($"期望 EthernetPortConfig，实际为 {config.GetType().Name}");

        if (ethernetPortConfig.Address.PortType == PortType.Tcp)
            return new TcpPortAdapter(ethernetPortConfig, loggerFactory.CreateLogger<TcpPortAdapter>());

        return Result.Fail("配置错误");
    }
}