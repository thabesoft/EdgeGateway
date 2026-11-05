using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Devices;


public interface IDevice : ILifecycle, IAsyncDisposable
{
    DeviceId Id { get; }
    ProtocolType ProtocolType { get; }


    IReadOnlyCollection<ISubscription> Subscriptions { get; }
    ValueTask<ISubscription<T>> AddSubscriptionAsync<T>(SubscriptionDescriptor<T> subscriptionDescriptor) where T : notnull;
}