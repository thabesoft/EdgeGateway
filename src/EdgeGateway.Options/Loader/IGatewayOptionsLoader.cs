using EdgeGateway.Devices;
using EdgeGateway.Dto;
using EdgeGateway.Options;
using EdgeGateway.Ports;
using EdgeGateway.Protocols;
using EdgeGateway.Serialization;
using FluentResults;
using Microsoft.Extensions.Options;
using System.IO.Ports;
using System.Net;

namespace EdgeGateway.Loader;

public interface IGatewayOptionsLoader
{
    ValueTask<Result<GatewayOptions>> LoadAsync(CancellationToken cancellationToken = default);
}

internal sealed class GatewayOptionsLoader(IOptions<GatewaySourceOptions> options, IGatewayOptionsSerializer serializer) : IGatewayOptionsLoader
{
    public async ValueTask<Result<GatewayOptions>> LoadAsync(CancellationToken cancellationToken = default)
    {
        var optionsFile = options.Value.Path;
        if (!File.Exists(optionsFile)) return Result.Fail($"网关配置文件不存在: [{optionsFile}]");


        using var fs = File.OpenRead(optionsFile);
        var result = await serializer.DeserializeAsync(fs, cancellationToken);
        if (result.IsFailed) return Result.Fail("网关配置文件反序列化失败").WithErrors(result.Errors);


        return Map(result.Value);
    }

    private static Result<GatewayOptions> Map(GatewayOptionsDto dto)
    {
        return new GatewayOptions()
        {
            Ports = [.. dto.Ports.Select(Map)],
            Devices = [.. dto.Devices.Select(Map)]
        };
    }


    // 端口 Dto -> Options
    private static IPortOptions Map(PortOptionsDto dto)
    {
        if (dto is SerialPortOptionsDto serialPort)
        {
            return SerialPortOptions.Builder(serialPort.PortName)
                .SetParity(Enum.Parse<Parity>(serialPort.Parity))
                .SetBaudRate(serialPort.BaudRate)
                .SetStopBits(StopBitsFromNumber(serialPort.StopBits))
                .SetDataBits(serialPort.DataBits)
                .Build();
        }

        if (dto is TcpPortOptionsDto tcp)
        {
            return EthernetPortOptions.Tcp(IPEndPoint.Parse(tcp.Remote))
                .Build();
        }

        if (dto is UdpPortOptionsDto udp)
        {
            return EthernetPortOptions.Udp(IPEndPoint.Parse(udp.Remote))
                .Build();
        }

        throw new NotSupportedException($"不支持的端口配置类型: {dto.GetType()}");


        // 停止位数字转枚举
        static StopBits StopBitsFromNumber(double value) => value switch
        {
            1 => StopBits.One,
            1.5 => StopBits.OnePointFive,
            2 => StopBits.Two,
            _ => StopBits.None
        };
    }

    // 设备 Dto -> Options
    private static DeviceOptions Map(DeviceOptionsDto dto)
    {
        var protocolType = ProtocolType.Parse(dto.Protocol);
        var portType = PortType.Parse(dto.PortAddress.Type);
        var portAddress = PortAddress.Create(portType, dto.PortAddress.Location);

        return DeviceOptions.Create(protocolType, portAddress);
    }
}