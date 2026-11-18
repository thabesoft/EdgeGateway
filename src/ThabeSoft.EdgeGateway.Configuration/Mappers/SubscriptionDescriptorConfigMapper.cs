using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Subscriptions;


public sealed class SubscriptionDescriptorConfigMapper<T>(
        IConfigMapper<TriggerDescriptor> trigegrMapper,
        IConfigMapper<TagDescriptor<T>> tagDescriptorMapper
    ) : IConfigMapper<SubscriptionDescriptor<T>>
    where T : notnull
{
    public bool CanMap(IConfigNode root)
    {
        return true;
    }

    public SubscriptionDescriptor<T> Map(IConfigNode root)
    {
        var id = SubscriptionId.From(root.GetValue("Id").Required("缺少订阅Id"));
        var trigger = trigegrMapper.Map(root.GetChild("Trigger").Required("缺少触发器配置"));
        var tag =  tagDescriptorMapper.Map(root.GetChild("Tag").Required("缺少标签"));

        return new() { Id = id, Trigger = trigger, Tag = tag };
    }
}