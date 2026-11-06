using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Configuration;


public interface IGatewayConfigurator
{
    IPortConfigurator AddPort(ITransportDescriptor transport);
}
public interface IPortConfigurator : IGatewayConfigurator
{
    IDeviceConfigurator AddDevice(DeviceId deviceId, IProtocolDescriptor protocol);
}
public interface IDeviceConfigurator : IPortConfigurator
{
    ISubscriptionConfigurator<T> AddSubscription<T>(TagDescriptor<T> tagDescriptor, ITriggerDescriptor triggerDescriptor) where T : notnull;
}
public interface ISubscriptionConfigurator<T> : IDeviceConfigurator
    where T : notnull
{
    ISubscriptionConfigurator<T> Observable(Action<IObservable<Reading<T>>> configureObservable);
}

public delegate void GatewayConfiguratorHandler(IGatewayConfigurator configurator);