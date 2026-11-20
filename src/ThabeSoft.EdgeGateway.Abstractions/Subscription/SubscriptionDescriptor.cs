using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Primitives.Guard;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Subscription;


/// <summary>
/// 订阅描述
/// </summary>
public interface ISubscriptionDescriptor
{
    SubscriptionId Id { get; }
    TriggerDescriptor Trigger { get; }
    ITagDescriptor Tag { get; }

    ValueTask<ISubscription> AddToDeviceAsync(IDevice device, CancellationToken cancellationToken = default);
}

/// <summary>
/// 订阅描述
/// </summary>
public sealed record class SubscriptionDescriptor<T> : ISubscriptionDescriptor
    where T : notnull
{
    public SubscriptionId Id { get; init => field = value.Required(); } = SubscriptionId.Create();
    public required TagDescriptor<T> Tag { get; init => field = value.Required(); }
    public required TriggerDescriptor Trigger { get; init => field = value.Required(); }
    ITagDescriptor ISubscriptionDescriptor.Tag => Tag;


    public ValueTask<ISubscription<T>> AddToDeviceAsync(IDevice device, CancellationToken cancellationToken = default) =>
        device.AddSubscriptionAsync(this, cancellationToken);
    async ValueTask<ISubscription> ISubscriptionDescriptor.AddToDeviceAsync(IDevice device, CancellationToken cancellationToken) =>
       await AddToDeviceAsync(device, cancellationToken);
}

public static class SubscriptionDescriptor
{
    public static SubscriptionDescriptor<T> Create<T>(SubscriptionId id, TagDescriptor<T> tag, TriggerDescriptor trigger) where T : notnull
    {
        return new() { Id = id, Tag = tag, Trigger = trigger };
    }

    public static SubscriptionDescriptor<T> Create<T>(TagDescriptor<T> tag, TriggerDescriptor trigger) where T : notnull
    {
        return Create(SubscriptionId.Create(), tag, trigger);
    }
}