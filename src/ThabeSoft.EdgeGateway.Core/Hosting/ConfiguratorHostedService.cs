using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Hosting;


internal class ConfiguratorHostedService(IGateway gateway, IEnumerable<GatewayConfiguratorHandler> configuratorHandlers) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var builder = new GatewayConfigurator(gateway);

        foreach (var i in configuratorHandlers)
        {
            i.Invoke(builder);
        }
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}


internal class GatewayConfigurator(IGateway gateway) :
    IGatewayConfigurator
{
    public IPortConfigurator AddPort(ITransportDescriptor transport)
    {
        var port = gateway.AddPortAsync(transport).Result;
        return new PortConfigurator(gateway, port);
    }
}
internal class PortConfigurator(IGateway gateway, IPort port) :
    GatewayConfigurator(gateway), IPortConfigurator
{
    private readonly IGateway _gateway = gateway;

    public IDeviceConfigurator AddDevice(DeviceId deviceId, IProtocolDescriptor protocol)
    {
        var device = port.AddDeviceAsync(DeviceDescriptor.Create(deviceId, protocol)).Result;
        return new DeviceConfigurator(_gateway, port, device);
    }
}
internal class DeviceConfigurator(IGateway gateway, IPort port, IDevice device) :
    PortConfigurator(gateway, port), IDeviceConfigurator
{
    private readonly IGateway _gateway = gateway;
    private readonly IPort _port = port;


    public ISubscriptionConfigurator<T> AddSubscription<T>(TagDescriptor<T> tagDescriptor, ITriggerDescriptor triggerDescriptor) where T : notnull
    {
        var subscription = device.AddSubscriptionAsync(tagDescriptor, triggerDescriptor).Result;
        return new SubscriptionConfigurator<T>(_gateway, _port, device, subscription);
    }
}
internal sealed class SubscriptionConfigurator<T>(IGateway gateway, IPort port, IDevice device, ISubscription<T> subscription) :
    DeviceConfigurator(gateway, port, device), ISubscriptionConfigurator<T>
    where T : notnull
{
    public ISubscriptionConfigurator<T> OnValue(Action<T> action)
    {
        subscription.Tag.Subscribe(action);
        return this;
    }
}