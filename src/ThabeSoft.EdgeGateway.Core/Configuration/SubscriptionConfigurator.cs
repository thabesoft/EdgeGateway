using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Subscription;

namespace ThabeSoft.EdgeGateway.Configuration;


internal sealed class SubscriptionConfigurator<T>(IGateway gateway, IPort port, IDevice device, ISubscription<T> subscription) :
    DeviceConfigurator(gateway, port, device), ISubscriptionConfigurator<T>
    where T : notnull
{
    public ISubscriptionConfigurator<T> Observable(Action<IObservable<Reading<T>>> configureObservable)
    {
        configureObservable.Invoke(subscription);
        return this;
    }
}