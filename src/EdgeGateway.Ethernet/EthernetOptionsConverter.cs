using EdgeGateway.Options;
using EdgeGateway.Options.Loader;
using EdgeGateway.Ports;

namespace EdgeGateway.Ethernet;


internal sealed class TcpPortOptionsConverter : IPortOptionsConverter
{
    public bool CanConvert(PortOptionsDto dto)
    {
        return dto is TcpPortOptionsDto;
    }

    public IPortOptions Convert(PortOptionsDto source)
    {
        if (source is not TcpPortOptionsDto dto)
            throw new NotSupportedException($"不支持的配置类型: {source.GetType().Name}");

        return EthernetPortOptions.Tcp(dto.Remote)
            .SetLocal(dto.Local)
            .Build();
    }
}

internal sealed class UdpPortOptionsConverter : IPortOptionsConverter
{
    public bool CanConvert(PortOptionsDto dto)
    {
        return dto is UdpPortOptionsDto;
    }

    public IPortOptions Convert(PortOptionsDto source)
    {
        if (source is not UdpPortOptionsDto dto)
            throw new NotSupportedException($"不支持的配置类型: {source.GetType().Name}");

        return EthernetPortOptions.Udp(dto.Remote)
            .SetLocal(dto.Local)
            .Build();
    }
}