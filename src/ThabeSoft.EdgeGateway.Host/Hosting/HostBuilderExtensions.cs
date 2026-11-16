using Microsoft.Extensions.Hosting;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Nodes;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Compatibility;
using ThabeSoft.EdgeGateway.DependencyInjection;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Subscription;

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
    public ImmutableArray<PortConfig> Ports { get; init => field = value.ToImmutableArrayOrEmpty(); } = [];
}

class PortConfig
{
    public required PortDescriptor Descriptor { get; init => field = value.Required(); }
    public ImmutableArray<DeviceConfig> Devices { get; init => field = value.ToImmutableArrayOrEmpty(); } = [];
}

class DeviceConfig
{
    public required DeviceDescriptor Descriptor { get; init => field = value.Required(); }
    public ImmutableArray<SubscriptionConfig> Subscriptions { get; init => field = value.ToImmutableArrayOrEmpty(); } = [];
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
        ArgumentNullException.ThrowIfNull(root);

        foreach (var port in root.GetByPath("Ports")?.AsArray() ?? [])
        {
            ParsePort(port.Required("端口配置为空")!);
        }

        return null!;
    }


    private void ParsePort(JsonNode root)
    {
        var descriptorNode = root.GetByPath("Descriptor").Required("端口描述符不存在")!;
        var portId = PortId.From(descriptorNode.GetByPath("Id").Required("缺少端口Id")!.ToString());
        
        var transportNode = descriptorNode.GetByPath("Transport").Required("缺少端口传输配置");
        transportNode.GetByPath("Type");
        
    }


    
}