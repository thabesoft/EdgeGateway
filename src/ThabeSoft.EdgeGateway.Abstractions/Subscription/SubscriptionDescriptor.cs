using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Subscription;


/// <summary>
/// 订阅描述
/// </summary>
public sealed class SubscriptionDescriptor<T> where T : notnull
{
    public required SubscriptionId Id { get; init; }
    public required TagDescriptor<T> Tag { get; init; }
    public required ITriggerDescriptor Trigger { get; init; }


    internal SubscriptionDescriptor() { }

}

public static class SubscriptionDescriptor
{
    public static SubscriptionDescriptor<T> Create<T>(SubscriptionId id, TagDescriptor<T> tag, ITriggerDescriptor trigger) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(tag);
        ArgumentNullException.ThrowIfNull(trigger);

        return new() { Id = id, Trigger = trigger, Tag = tag };
    }

    public static SubscriptionDescriptor<T> Create<T>(TagDescriptor<T> tag, ITriggerDescriptor trigger) where T : notnull =>
        Create(SubscriptionId.Create(), tag, trigger);
}
