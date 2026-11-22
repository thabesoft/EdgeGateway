using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.Protocols.Accessor;
using ThabeSoft.Triggers;

namespace ThabeSoft.EdgeGateway.Subscription;


/// <summary>
/// 订阅描述
/// </summary>
public interface ISubscriptionDescriptor
{
    SubscriptionId Id { get; }
    TriggerDescriptor Trigger { get; }
    IDataAccessDescriptor Tag { get; }
    LifecycleStateMachineDescriptor StateMachine { get; }

    ValueTask<ISubscription> AddToDeviceAsync(IDevice device, CancellationToken cancellationToken = default);
}

/// <summary>
/// 订阅描述
/// </summary>
public sealed record class SubscriptionDescriptor<T> : ISubscriptionDescriptor
    where T : notnull
{
    public SubscriptionId Id { get; init => field = value.Required(); } = SubscriptionId.Create();
    public required DataAccessDescriptor<T> Tag { get; init => field = value.Required(); }
    public required TriggerDescriptor Trigger { get; init => field = value.Required(); }
    public LifecycleStateMachineDescriptor StateMachine { get; init => field = value.Required(); } = Lifecycle.StrictStateMachine();

    IDataAccessDescriptor ISubscriptionDescriptor.Tag => Tag;


    public ValueTask<ISubscription<T>> AddToDeviceAsync(IDevice device, CancellationToken cancellationToken = default) =>
        device.AddSubscriptionAsync(this, cancellationToken);
    async ValueTask<ISubscription> ISubscriptionDescriptor.AddToDeviceAsync(IDevice device, CancellationToken cancellationToken) =>
       await AddToDeviceAsync(device, cancellationToken);
}

public static class SubscriptionDescriptor
{
    public static SubscriptionDescriptor<T> Create<T>(SubscriptionId id, DataAccessDescriptor<T> tag, TriggerDescriptor trigger) where T : notnull
    {
        return new() { Id = id, Tag = tag, Trigger = trigger };
    }

    public static SubscriptionDescriptor<T> Create<T>(DataAccessDescriptor<T> tag, TriggerDescriptor trigger) where T : notnull
    {
        return Create(SubscriptionId.Create(), tag, trigger);
    }
}