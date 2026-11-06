using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Devices;


public interface IDevice : ILifecycle, IAsyncDisposable
{
    DeviceId Id { get; }
    ProtocolType ProtocolType { get; }


    IReadOnlyCollection<ISubscription> Subscriptions { get; }
    ValueTask<ISubscription<T>> AddSubscriptionAsync<T>(SubscriptionDescriptor<T> subscriptionDescriptor) where T : notnull;
}

public static class DeviceExtensions
{
    extension(IDevice device)
    {
        public ValueTask<ISubscription<T>> AddSubscriptionAsync<T>(
                TagDescriptor<T> tagDescriptor,
                ITriggerDescriptor triggerDescriptor
            ) where T : notnull
        {
            var descriptor = SubscriptionDescriptor.Create(tagDescriptor, triggerDescriptor);
            return device.AddSubscriptionAsync(descriptor);
        }
    }
}