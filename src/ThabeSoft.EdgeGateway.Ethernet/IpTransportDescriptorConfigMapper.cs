using System.Net;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Ethernet;


internal sealed class IpTransportDescriptorConfigMapper(
        IConfigMapper<RetryDescriptor> retryDescriptorMapper
    ) : IConfigMapper<TransportDescriptor>
{
    public bool CanMap(IConfigNode root)
    {
        if (!Enum.TryParse<TransportType>(root["Type"]?.AsString(), true, out var enumValue))
            return false;

        return enumValue == TransportType.Ethernet;
    }

    public TransportDescriptor Map(IConfigNode root)
    {
        var type = root["Type"]?.AsString().Required("缺少传输类型")
             .ToEnum<TransportType>();

        if (type != TransportType.Ethernet)
            throw new InvalidOperationException("不支持");

        var ip = IPAddress.Parse(root.GetStringValue("Ip").Required("缺少Ip"));
        var port = root.GetStringValue("Port").Required("缺少端口").ToInt32();
        var ipProtocolType = root.GetStringValue("ProtocolType").Required("缺少Ip协议类型")
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
        var retryNode = root["Retry"];
        if (retryNode is null) return RetryBuilder.Default;

        return retryDescriptorMapper.Map(retryNode);
    }
}