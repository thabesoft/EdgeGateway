using ThabeSoft.EdgeGateway.Configuration.Nodes;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Subscriptions;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Configuration.Mappers;


public class PortConfigMapper(
        IConfigMapper<TransportDescriptor> transportMapper,
        IConfigMapper<DeviceConfig> deviceConfigMapper
        ) : IConfigMapper<PortConfig>
{
    public PortConfig Map(IConfigNode root)
    {
        var id = PortId.From(root.GetString("Id").Required("缺少端口Id"));
        var transport = transportMapper.Map(root.GetChild("Transport").Required("缺少传输配置"));
        var devices = root.GetChildren("Devices").Select(deviceConfigMapper.Map);

        return new() { Descriptor = new() { Transport = transport, Id = id }, Devices = [.. devices] };
    }
}


public class DeviceConfigMapper(
        IConfigMapper<ProtocolDescriptor> protocolMapper,
        IConfigMapper<SubscriptionConfig> subscriptionConfigMapper
        ) : IConfigMapper<DeviceConfig>
{
    public DeviceConfig Map(IConfigNode root)
    {
        var id = DeviceId.From(root.GetString("Id").Required("缺少设备Id"));
        var protocol = protocolMapper.Map(root.GetChild("Protocol").Required("缺少协议配置"));
        var subscriptions = root.GetChildren("Subscriptions").Select(subscriptionConfigMapper.Map);

        return new() { Descriptor = new() { Id = id, Protocol = protocol }, Subscriptions = [.. subscriptions] };
    }
}

public class SubscriptionConfigMapper(
        IConfigMapper<TriggerDescriptor> trigegrMapper,
        IConfigMapper<SubscriptionConfig> subscriptionConfigMapper
        ) : IConfigMapper<SubscriptionConfig>
{
    public SubscriptionConfig Map(IConfigNode root)
    {
        var id = SubscriptionId.From(root.GetString("Id").Required("缺少订阅Id"));
        var trigger = trigegrMapper.Map(root.GetChild("Trigger").Required("缺少触发器配置"));
        var subscriptions = root.GetChildren("Subscriptions").Select(subscriptionConfigMapper.Map);

        return new() { Descriptor = new() { Id = id, Trigger = trigger }, Subscriptions = [.. subscriptions] };
    }
}