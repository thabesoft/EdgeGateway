using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Devices;


public interface IDevice : ILifecycle, ILifecycleStateable
{
    DeviceId Id { get; }
    ProtocolType ProtocolType { get; }
    IReadOnlyDictionary<SubscriptionId, ISubscription> Subscriptions { get; }


    ValueTask<ISubscription<T>> AddSubscriptionAsync<T>(
        SubscriptionDescriptor<T> subscriptionDescriptor,
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
                TagDescriptor<T> tagDescriptor,
                ITriggerDescriptor triggerDescriptor,
                CancellationToken cancellationToken = default
            ) where T : notnull
        {
            var descriptor = SubscriptionDescriptor.Create(tagDescriptor, triggerDescriptor);
            return device.AddSubscriptionAsync(descriptor, cancellationToken);
        }
    }
}