using ThabeSoft.EdgeGateway.Compatibility;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Subscription;


/// <summary>
/// 订阅描述
/// </summary>
public abstract record class SubscriptionDescriptor
{
    public SubscriptionId Id { get; init => field = value.Required(); } = SubscriptionId.Create();
    public required ITriggerDescriptor Trigger { get; init => field = value.Required(); }



    public static SubscriptionDescriptor<T> Create<T>(SubscriptionId id, TagDescriptor<T> tag, ITriggerDescriptor trigger) where T : notnull =>
       new() { Id = id, Tag = tag, Trigger = trigger };
    public static SubscriptionDescriptor<T> Create<T>(TagDescriptor<T> tag, ITriggerDescriptor trigger) where T : notnull =>
        new() { Tag = tag, Trigger = trigger };
}


/// <summary>
/// 订阅描述
/// </summary>
public sealed record class SubscriptionDescriptor<T> : SubscriptionDescriptor
    where T : notnull
{
    public required TagDescriptor<T> Tag { get; init => field = value.Required(); }
}