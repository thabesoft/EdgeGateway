using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.App.Designs;

public sealed class DesignPort(ITransportDescriptor transportDescriptor, DesignGateway gateway) : ResilientLifecycleBase, IPort
{
    private bool _isDisposed;
    private readonly ConcurrentDictionary<DeviceId, IDevice> _devices = [];


    public ConnectionEndpoint Endpoint => transportDescriptor.Address;
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => _devices.AsReadOnly();


    public IDevice AddDevice(DeviceDescriptor descriptor)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var device = new DesignDevice(descriptor, this);
        _devices[device.Id] = device;
        return device;
    }
    public async ValueTask RemoveDeviceAsync(DeviceId id, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (!_devices.TryRemove(id, out var device)) return;
        await device.DisposeAsync();
    }


    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        foreach (var i in _devices.Values)
        {
            await i.StartAsync(cancellationToken);
        }
    }
    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        foreach (var i in _devices.Values)
        {
            await i.StopAsync(cancellationToken);
        }
    }
    protected override async ValueTask OnDisposeAsync()
    {
        await gateway.RemovePortAsync(Endpoint);

        foreach (var i in _devices.Values)
        {
            await i.DisposeAsync();
        }
    }
}
