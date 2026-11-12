using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Observable;
using ThabeSoft.EdgeGateway.Subscription;

namespace ThabeSoft.EdgeGateway.Devices;

public interface IObservableDevice : IDevice, IObservableLifecycleState
{
    new IObservableDictionary<SubscriptionId, IObservablSubscription> Subscriptions { get; }
}
