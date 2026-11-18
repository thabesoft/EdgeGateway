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
        if (!Enum.TryParse<TransportType>(root.GetValue("Type"), true, out var enumValue))
            return false;

        return enumValue == TransportType.Ethernet;
    }

    public TransportDescriptor Map(IConfigNode root)
    {
        var type = root.GetValue("Type").Required("缺少类型")
             .ToEnum<TransportType>();

        if (type != TransportType.Ethernet)
            throw new InvalidOperationException("不支持");


        var ipProtocolType = root.GetValue("ProtocolType").Required("")
              .ToEnum<IpProtocolType>();

        var ip = IPAddress.Parse(root.GetValue("Ip").Required(""));
        var port = root.GetValue("Port").Required("").ToInt32();
        var retry = retryDescriptorMapper.Map(root.GetChild("Retry").Required());

        return new IpTransportDescriptor()
        {
            IP = ip,
            Port = port,
            ProtocolType = ipProtocolType,
            Retry = retry
        };
    }
}