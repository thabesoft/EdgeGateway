using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Ports;


public class PortConfigMapper(
        IConfigMapper<PortDescriptor> portDescriptortMapper,
        IConfigMapper<DeviceConfig> deviceConfigMapper
        ) : IConfigMapper<PortConfig>
{
    public bool CanMap(IConfigNode root)
    {
        return true;
    }

    public PortConfig Map(IConfigNode root)
    {
        var descriptor = portDescriptortMapper.Map(root);
        var devices = root.GetChildren("Devices").Select(deviceConfigMapper.Map);

        return new() { Descriptor = descriptor, Devices = [.. devices] };
    }
}