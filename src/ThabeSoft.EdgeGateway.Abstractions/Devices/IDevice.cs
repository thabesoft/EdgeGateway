using ThabeSoft.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.Triggers;
using ThabeSoft.Protocols.Accessor;

namespace ThabeSoft.EdgeGateway.Devices;


public interface IDevice : ILifecycleWithState
{
    DeviceId Id { get; }
    ProtocolType ProtocolType { get; }
    IReadOnlyDictionary<SubscriptionId, ISubscription> Subscriptions { get; }


    ValueTask<ISubscription<T>> AddSubscriptionAsync<T>(
        SubscriptionDescriptor<T> subscription,
        CancellationToken cancellationToken = default
    ) where T : notnull;

    ValueTask<bool> RemoveSubscriptionAsync(
        SubscriptionId subscriptionId,
        CancellationToken cancellationToken = default);
}

public static class DeviceExtensions
{
    extension(IDevice device)
    {
        public ValueTask<ISubscription<T>> AddSubscriptionAsync<T>(
                SubscriptionId id,
                DataAccessDescriptor<T> tag,
                TriggerDescriptor trigger,
                CancellationToken cancellationToken = default
            ) where T : notnull
        {
            var descriptor = new SubscriptionDescriptor<T>
            {
                Id = id,
                Tag = tag,
                Trigger = trigger
            };
            return device.AddSubscriptionAsync(descriptor, cancellationToken);
        }

        public ValueTask<ISubscription<T>> AddSubscriptionAsync<T>(
                DataAccessDescriptor<T> tag,
                TriggerDescriptor trigger,
                CancellationToken cancellationToken = default
            ) where T : notnull
        {
            return device.AddSubscriptionAsync(SubscriptionId.Create(), tag, trigger, cancellationToken);
        }
    }
}