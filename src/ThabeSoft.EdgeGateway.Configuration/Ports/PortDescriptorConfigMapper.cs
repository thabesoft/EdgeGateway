using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Ports;

public sealed class PortDescriptorConfigMapper(
        IConfigMapper<TransportDescriptor> transportMapper
    ) : IConfigMapper<PortDescriptor>
{
    public bool CanMap(IConfigNode root)
    {
        return true;
    }

    public PortDescriptor Map(IConfigNode root)
    {
        var id = PortId.From(root.GetValue("Id").Required("缺少端口Id"));
        var transport = transportMapper.Map(root.GetChild("Transport").Required("缺少传输配置"));

        return new() { Id = id, Transport = transport };
    }
}