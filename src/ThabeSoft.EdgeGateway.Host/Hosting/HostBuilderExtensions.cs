using Microsoft.Extensions.Hosting;
using System.Collections.Immutable;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Compatibility;
using ThabeSoft.EdgeGateway.DependencyInjection;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Hosting;


public static class HostBuilderExtensions
{
    extension(IHostBuilder builder)
    {
        public IHostBuilder UseEdgeGateway()
        {
            // Services
            builder.ConfigureServices((_, descriptors) =>
            {
                descriptors.AddEdgeGateway();
                descriptors.AddEdgeGatewayEthernet();
                descriptors.AddEdgeGatewayModbus();
                descriptors.AddEdgeGatewaySerial();

            });

            return builder;
        }
    }
}


class GatewayConfig()
{
    public ImmutableArray<PortConfig> Ports { get; init => field = [.. value.WhereNotNull()]; } = [];
}

class PortConfig
{
    public required PortDescriptor Descriptor { get; init => field = value.Required(); }
    public ImmutableArray<DeviceConfig> Devices { get; init => field = [.. value.WhereNotNull()]; } = [];
}

class DeviceConfig
{
    public required DeviceDescriptor Descriptor { get; init => field = value.Required(); }
    public ImmutableArray<SubscriptionConfig> Subscriptions { get; init => field = [.. value.WhereNotNull()]; } = [];
}

class SubscriptionConfig
{
    public required SubscriptionDescriptor Descriptor { get; init => field = value.Required(); }
}




interface IGatewayConfigDeserializer
{
    ValueTask<GatewayConfig> DeserializeAsync(Stream stream, CancellationToken cancellationToken = default);
}


class IGatewayJsonConfigDeserializer : IGatewayConfigDeserializer
{
    public async ValueTask<GatewayConfig> DeserializeAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var root = await JsonNode.ParseAsync(stream, cancellationToken: cancellationToken);
        return AAA.AsGatewayConfig(root.Required("网关配置不可为空"));
    }
}



internal static class AAA
{
    public static GatewayConfig AsGatewayConfig(JsonNode root)
    {
        var ports = root.GetByPath("Ports")
            ?.AsArray()
            .Where(x => x is not null)
            .Select(x => x!.AsPortConfig())
            ?? [];

        return new GatewayConfig() { Ports = [.. ports] };
    }


    private static PortConfig AsPortConfig(this JsonNode root)
    {
        var descriptor = root.GetByPath("Descriptor").Required("端口描述符不存在")!.AsPortDescriptor();
        return new() { Descriptor = descriptor, Devices = [] };
    }
    private static PortDescriptor AsPortDescriptor(this JsonNode root)
    {
        var portId = PortId.From(root.GetByPath("Id").Required("缺少端口Id")!.ToString());
        var transport = AsTransportDescriptor(root.GetByPath("Transport").Required("缺少端口传输配置")!);

        return new() { Id = portId, Transport = transport };
    }
    private static TransportDescriptor AsTransportDescriptor(this JsonNode root)
    {
        var transportType = root.GetValueStringByPath("Type").Required("缺少传输类型")
            .ToEnum<TransportType>();

        if (transportType == TransportType.Ethernet)
        {
            var ipProtocolType = root.GetValueStringByPath("ProtocolType").Required("缺少Ip协议类型")
                .ToEnum<IpProtocolType>();

            var ip = root.GetValueStringByPath("Ip").Required("缺少Ip地址")
                .ToIpAddress();

            var port = root.GetValueStringByPath("Port").Required("缺少端口")
                .ToInt32()
                .NonNegative("端口不可为负数");

            var retry = root.GetByPath("Retry")?.AsRetryDescriptor()
                ?? Retry.Default;


            return new IpTransportDescriptor()
            {
                ProtocolType = ipProtocolType,
                IP = ip,
                Port = port,
                Retry = retry
            };
        }

        if (transportType == TransportType.Serial)
        {
            var portName = root.GetValueStringByPath("PortName").Required("缺少串口名称")
                .PortName()!;

            var baudRate = root.GetValueStringByPath("BaudRate").Required("缺少波特率")
                .ToInt32()
                .StandardBaudRate();

            var dataBits = root.GetValueStringByPath("DataBits").Required("缺少数据位")
                .ToInt32()
                .DataBits();

            var parity = root.GetValueStringByPath("Parity").Required("缺少校验值")
                .ToEnum<SerialParity>();

            var stopBits = root.GetValueStringByPath("StopBits").Required("缺少停止位")
                .ToEnum<SerialStopBits>();

            var retry = root.GetByPath("Retry")?.AsRetryDescriptor()
                ?? Retry.Default;

            return new SerialTransportDescriptor()
            {
                PortName = portName,
                BaudRate = baudRate,
                DataBits = dataBits,
                Parity = parity,
                StopBits = stopBits,
                Retry = retry
            };
        }


        throw new InvalidOperationException("不支持的传输描述符");
    }
    private static RetryDescriptor AsRetryDescriptor(this JsonNode root)
    {
        var startInterval = root.GetValueStringByPath("StartInterval").Required("缺少起始间隔")
            .ToTimeSpan();

        var maxInterval = root.GetValueStringByPath("MaxInterval").Required("缺少最大间隔")
            .ToTimeSpan();

        var backoffMultiplier = root.GetValueStringByPath("BackoffMultiplier").Required("缺少退避倍数")
            .ToDouble()
            .AtLeast(1);

        return new(startInterval, maxInterval)
        {
            BackoffMultiplier = backoffMultiplier
        };
    }
}