using System.Net;
using ThabeSoft.Configuration;
using ThabeSoft.Configuration.Nodes;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.Transports;

namespace ThabeSoft.Transports.Ethernet;


internal sealed class IpTransportDescriptorConfigMapper(
        IConfigMapper<RetryDescriptor> retryDescriptorMapper
    ) : IConfigMapper<TransportDescriptor>
{
    public bool CanMap(IConfigNode root)
    {
        if (!Enum.TryParse<TransportType>(root.GetByPath("Type")?.AsString(), true, out var enumValue))
            return false;

        return enumValue == TransportType.Ethernet;
    }

    public TransportDescriptor Map(IConfigNode root)
    {
        var type = root.GetByPath("Type")?.AsString().Required("缺少传输类型")
             .ToEnum<TransportType>();

        if (type != TransportType.Ethernet)
            throw new InvalidOperationException("不支持");

        var ip = IPAddress.Parse(root.GetStringByPath("Ip").Required("缺少Ip"));
        var port = root.GetStringByPath("Port").Required("缺少端口").ToInt32();
        var ipProtocolType = root.GetStringByPath("ProtocolType").Required("缺少Ip协议类型")
            .ToEnum<IpProtocolType>();

        return new IpTransportDescriptor()
        {
            IP = ip,
            Port = port,
            ProtocolType = ipProtocolType,
            Retry = MapRetryOrDefault(root)
        };
    }

    private RetryDescriptor MapRetryOrDefault(IConfigNode root)
    {
        var retryNode = root.GetByPath("Retry");
        if (retryNode is null) return RetryBuilder.Default;

        return retryDescriptorMapper.Map(retryNode);
    }
}