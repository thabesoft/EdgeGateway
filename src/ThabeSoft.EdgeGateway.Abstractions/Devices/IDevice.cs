using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Devices;


public interface IDevice : IStatefulLifecycle, IAsyncDisposable
{
    DeviceId Id { get; }
    ProtocolType ProtocolType { get; }


    IReadOnlyDictionary<SubscriptionId, ISubscription> Subscriptions { get; }
    ISubscription<T> AddSubscription<T>(SubscriptionDescriptor<T> subscriptionDescriptor) where T : notnull;
}

public static class DeviceExtensions
{
    extension(IDevice device)
    {
        public ISubscription<T> AddSubscription<T>(
                TagDescriptor<T> tagDescriptor,
                ITriggerDescriptor triggerDescriptor
            ) where T : notnull
        {
            var descriptor = SubscriptionDescriptor.Create(tagDescriptor, triggerDescriptor);
            return device.AddSubscription(descriptor);
        }
    }
}