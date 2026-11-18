using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Configuration.Mappers;

public sealed class PortDescriptorConfigMapper(
       IEnumerable<IConfigMapper<TransportDescriptor>> transportMappers
    ) : IConfigMapper<PortDescriptor>
{
    public bool CanMap(IConfigNode root)
    {
        return true;
    }

    public PortDescriptor Map(IConfigNode root)
    {
        var transportNode = root.GetChild("Transport").Required("缺少传输配置");
        var transport = transportMappers.FirstOrDefault(x => x.CanMap(transportNode))
            .Map(transportNode);

        var id = PortId.From(root.GetValue("Id").Required("缺少端口Id"));
        return new() { Id = id, Transport = transport };
    }
}