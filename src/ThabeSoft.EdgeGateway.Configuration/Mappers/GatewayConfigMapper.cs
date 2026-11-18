using ThabeSoft.EdgeGateway.Configuration.Models;

namespace ThabeSoft.EdgeGateway.Configuration.Mappers;


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