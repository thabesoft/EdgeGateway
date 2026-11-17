using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway;


public class GatewayConfigMapper(
        IConfigMapper<PortConfig> portConfigMapper
    ) : IConfigMapper<GatewayConfig>
{
    public bool CanMap(IConfigNode root)
    {
        return true;
    }

    public GatewayConfig Map(IConfigNode root)
    {
        var devices = root.GetChildren("Ports").Select(portConfigMapper.Map);
        return new() { Ports = [.. devices] };
    }
}