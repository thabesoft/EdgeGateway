using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Configuration;


internal class DeviceConfigurator(IGateway gateway, IPort port, IDevice device) :
    PortConfigurator(gateway, port), IDeviceConfigurator
{
    private readonly IGateway _gateway = gateway;
    private readonly IPort _port = port;


    public ISubscriptionConfigurator<T> AddSubscription<T>(TagDescriptor<T> tagDescriptor, ITriggerDescriptor triggerDescriptor) where T : notnull
    {
        var subscription = device.AddSubscription(tagDescriptor, triggerDescriptor);
        return new SubscriptionConfigurator<T>(_gateway, _port, device, subscription);
    }
}
