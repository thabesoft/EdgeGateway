using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Configuration.Mappers;

public sealed class DeviceDescriptorConfigMapper(
        IConfigMapper<ProtocolDescriptor> protocolMapper
    ) : IConfigMapper<DeviceDescriptor>
{
    public bool CanMap(IConfigNode root)
    {
        return true;
    }
    public DeviceDescriptor Map(IConfigNode root)
    {
        var id = DeviceId.From(root.GetValue("Id").Required("缺少设备Id"));
        var protocol = protocolMapper.Map(root.GetChild("Protocol").Required("缺少协议配置"));

        return new() { Id = id, Protocol = protocol };
    }
}