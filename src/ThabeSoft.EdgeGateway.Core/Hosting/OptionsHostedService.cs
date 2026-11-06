using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Hosting;

internal class OptionsHostedService(IGateway gateway, IEnumerable<Op> Ops) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var builder = new GatewayOptionsConfiguratorV2(gateway);

        foreach (var i in Ops)
        {
            i.Invoke(builder);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class GatewayOptionsConfiguratorV2(IGateway gateway) :
    IGatewayOptionsConfiguratorV2
{
    public IPortOptionsConfiguratorV2 AddPort(ITransportDescriptor transport)
    {
        var port = gateway.AddPortAsync(transport).Result;
        return new PortOptionsConfiguratorV2(gateway, port);
    }
}
internal class PortOptionsConfiguratorV2(IGateway gateway, IPort port) :
    GatewayOptionsConfiguratorV2(gateway), IPortOptionsConfiguratorV2
{
    private readonly IGateway _gateway = gateway;

    public IDeviceOptionsConfiguratorV2 AddDevice(DeviceId deviceId, IProtocolDescriptor protocol)
    {
        var device = port.AddDeviceAsync(DeviceDescriptor.Create(deviceId, protocol)).Result;
        return new DeviceOptionsConfiguratorV2(_gateway, port, device);
    }
}
internal class DeviceOptionsConfiguratorV2(IGateway gateway, IPort port, IDevice device) :
    PortOptionsConfiguratorV2(gateway, port), IDeviceOptionsConfiguratorV2
{
    private readonly IGateway _gateway = gateway;
    private readonly IPort _port = port;


    public ITagOptionsConfiguratorV2<T> AddSubscription<T>(TagDescriptor<T> tagDescriptor, ITriggerDescriptor triggerDescriptor) where T : notnull
    {
        var subscription = device.AddSubscriptionAsync(tagDescriptor, triggerDescriptor).Result;
        return new TagOptionsConfiguratorV2<T>(_gateway, _port, device, subscription);
    }
}
internal sealed class TagOptionsConfiguratorV2<T>(IGateway gateway, IPort port, IDevice device, ISubscription<T> subscription) :
    DeviceOptionsConfiguratorV2(gateway, port, device), ITagOptionsConfiguratorV2<T> where T : notnull
{
    public ITagOptionsConfiguratorV2<T> OnValue(Action<T> action)
    {
        subscription.Tag.Subscribe(action);
        return this;
    }
}