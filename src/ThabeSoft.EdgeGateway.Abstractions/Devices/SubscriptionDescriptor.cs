using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Polling;

/// <summary>
/// 订阅描述
/// </summary>
public sealed class SubscriptionDescriptor<T> where T : notnull
{
    public required TagDescriptor<T> Tag { get; init; }
    public required ITriggerDescriptor Trigger { get; init; }


    internal SubscriptionDescriptor() { }

}

public static class SubscriptionDescriptor
{
    public static SubscriptionDescriptor<T> Create<T>(TagDescriptor<T> tag, ITriggerDescriptor trigger) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(tag);
        ArgumentNullException.ThrowIfNull(trigger);

        return new() { Trigger = trigger, Tag = tag };
    }
}
