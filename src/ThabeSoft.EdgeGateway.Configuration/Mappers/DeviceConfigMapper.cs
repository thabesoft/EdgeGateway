using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Configuration.Models;
using ThabeSoft.EdgeGateway.Devices;

namespace ThabeSoft.EdgeGateway.Configuration.Mappers;

public class DeviceConfigMapper(
        IConfigMapper<DeviceDescriptor> deviceDescriptorConfigMapper,
        IConfigMapper<SubscriptionConfig> subscriptionConfigMapper
        ) : IConfigMapper<DeviceConfig>
{
    public bool CanMap(IConfigNode root)
    {
        return true;
    }

    public DeviceConfig Map(IConfigNode root)
    {
        var descriptor = deviceDescriptorConfigMapper.Map(root);
        var subscriptions = root.GetChildren("Subscriptions").Select(subscriptionConfigMapper.Map);

        return new() { Descriptor = descriptor, Subscriptions = [.. subscriptions] };
    }
}