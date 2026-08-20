using FluentResults;
using Microsoft.Extensions.Logging;

namespace EdgeGateway.Ports.Ethernet;


public sealed class EthernetPortFactory(ILoggerFactory loggerFactory) : IPortFactory
{
    public bool CanHandle(IPortConfig config)
    {
        return config is EthernetPortConfig;
    }

    public Result<IPort> Create(IPortConfig config)
    {
        if (config is not EthernetPortConfig ethernetPortConfig)
            return new Error($"期望 EthernetPortConfig，实际为 {config.GetType().Name}");

        if (ethernetPortConfig.Type == EthernetType.Tcp)
            return new TcpPortAdapter(ethernetPortConfig, loggerFactory.CreateLogger<TcpPortAdapter>());

        return Result.Fail("配置错误");
    }
}