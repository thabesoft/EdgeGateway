using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Subscription;


/// <summary>
/// 订阅描述
/// </summary>
public sealed class SubscriptionDescriptor<T> : IEquatable<SubscriptionDescriptor<T>>
    where T : notnull
{
    public SubscriptionDescriptor(SubscriptionId id, TagDescriptor<T> tag, ITriggerDescriptor trigger)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(tag);
        ArgumentNullException.ThrowIfNull(trigger);

        Id = id;
        Tag = tag;
        Trigger = trigger;
    }
    public SubscriptionDescriptor(TagDescriptor<T> tag, ITriggerDescriptor trigger) : this(SubscriptionId.Create(), tag, trigger)
    {

    }


    public SubscriptionId Id { get; }
    public TagDescriptor<T> Tag { get; }
    public ITriggerDescriptor Trigger { get; }


    public bool Equals(SubscriptionDescriptor<T> other)
    {
        return Id.Equals(other.Id) && Tag.Equals(other.Tag) && Trigger.Equals(other.Trigger);
    }
    public override string ToString() => $"Subscription({Id}, {Tag}, {Trigger})";
}

public static class SubscriptionDescriptor
{
    public static SubscriptionDescriptor<T> Create<T>(SubscriptionId id, TagDescriptor<T> tag, ITriggerDescriptor trigger) where T : notnull =>
        new(id, tag, trigger);

    public static SubscriptionDescriptor<T> Create<T>(TagDescriptor<T> tag, ITriggerDescriptor trigger) where T : notnull =>
        new(tag, trigger);
}
