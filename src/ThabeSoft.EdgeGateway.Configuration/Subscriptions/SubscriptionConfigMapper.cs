using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Subscriptions;


public class SubscriptionConfigMapper(
        IConfigMapper<SubscriptionDescriptor> configMapper,
        IConfigMapper<TriggerDescriptor> trigegrMapper
        ) : IConfigMapper<SubscriptionConfig>
{
    public bool CanMap(IConfigNode root)
    {
        return true;
    }

    public SubscriptionConfig Map(IConfigNode root)
    {
        var descriptor = configMapper.Map(root);
        var trigger = trigegrMapper.Map(root.GetChild("Trigger").Required("缺少触发器配置"));
        var valueType = root.GetValue("Tag:Type").Required("缺少触发器配置")
            .ToEnum<TagValueType>();

        return new() { Descriptor = descriptor, Trigger = trigger, ValueType = valueType };
    }
}