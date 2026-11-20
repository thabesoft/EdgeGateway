using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.Configuration.Models;
using ThabeSoft.Configuration.Nodes;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.Primitives;

namespace ThabeSoft.Configuration.Mappers;


public class GatewayConfigMapper(
        IServiceProvider serviceProvider,
        IEnumerable<IConfigMapper<TransportDescriptor>> transportMappers,
        IEnumerable<IConfigMapper<ProtocolDescriptor>> protocolMappers,
        IEnumerable<IConfigMapper<TriggerDescriptor>> triggerMappers,
        IEnumerable<IConfigMapper<DataAddress>> dataAddressMappers
    ) : IConfigMapper<GatewayConfig>
{
    public bool CanMap(IConfigNode root)
    {
        return true;
    }

    public GatewayConfig Map(IConfigNode root)
    {
        return new()
        {
            Ports = [.. root.EnumerateChildrenByPath("Ports").Select(GetPortConfig)]
        };
    }

    private PortConfig GetPortConfig(IConfigNode root)
    {
        return new()
        {
            Descriptor = new()
            {
                Id = PortId.From(root.GetStringByPath("Id").Required("缺少端口Id")),
                Transport = MapFromCollection(transportMappers, root.GetByPath("Transport").Required("缺少传输配置"))
            },
            Devices = [.. root.EnumerateChildrenByPath("Devices").Select(GetDeviceConfig)]
        };
    }

    private DeviceConfig GetDeviceConfig(IConfigNode root)
    {
        return new()
        {
            Descriptor = new()
            {
                Id = DeviceId.From(root.GetStringByPath("Id").Required("缺少设备Id")),
                Protocol = MapFromCollection(protocolMappers, root.GetByPath("Protocol").Required("缺少协议配置"))
            },
            Subscriptions = [.. root.EnumerateChildrenByPath("Subscriptions").Select(GetSubscriptionConfig)]
        };
    }

    private SubscriptionConfig GetSubscriptionConfig(IConfigNode root)
    {
        var dataType = root.GetByPath("Codec.DataType")?.AsString().Required($"缺少数据类型: {root.GetPath()}")
             .ToEnum<DataType>();

        return new()
        {
            Descriptor = dataType switch
            {
                DataType.Bits => GetGenericDescriptor<BitArray>(),
                DataType.Bool => GetGenericDescriptor<bool>(),
                DataType.Byte => GetGenericDescriptor<byte>(),

                DataType.UInt16 => GetGenericDescriptor<ushort>(),
                DataType.Int16 => GetGenericDescriptor<short>(),
                DataType.Char => GetGenericDescriptor<char>(),

                DataType.UInt32 => GetGenericDescriptor<uint>(),
                DataType.Int32 => GetGenericDescriptor<int>(),
                DataType.Float => GetGenericDescriptor<float>(),

                DataType.UInt64 => GetGenericDescriptor<ulong>(),
                DataType.Int64 => GetGenericDescriptor<long>(),
                DataType.Double => GetGenericDescriptor<double>(),

                _ => throw new InvalidOperationException("不支持的数据类型")
            }
        };


        SubscriptionDescriptor<T> GetGenericDescriptor<T>() where T : notnull
        {
            return new()
            {
                Id = SubscriptionId.From(root.GetStringByPath("Id").Required("缺少订阅Id")),
                Trigger = MapFromCollection(triggerMappers, root.GetByPath("Trigger").Required("缺少触发器")),
                Tag = new
                (
                    address: MapFromCollection(dataAddressMappers, root.GetByPath("DataAddress").Required("缺少数据地址")),
                    codec: GetCodecDescriptor<T>(root)
                )
            };
        }

        CodecDescriptor<T> GetCodecDescriptor<T>(IConfigNode root) where T : notnull
        {
            var codecs = serviceProvider.GetServices<IConfigMapper<CodecDescriptor<T>>>();
            return MapFromCollection(codecs, root.GetByPath("Codec").Required("缺少数据编码器"));
        }
    }


    private static T MapFromCollection<T>(IEnumerable<IConfigMapper<T>> mappers, IConfigNode node)
    {
        var mapper = mappers.FirstOrDefault(x => x.CanMap(node))
            ?? throw new InvalidOperationException($"节点  ({node}) 无法映射, 没有可执行的映射器");

        return mapper.Map(node);
    }
}