using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.App.Designs;

public sealed class DesignPort(ITransportDescriptor transportDescriptor, DesignGateway gateway) : IPort
{
    private bool _isDisposed;
    private readonly ConcurrentDictionary<DeviceId, IDevice> _devices = [];


    public ConnectionEndpoint Endpoint => transportDescriptor.Address;
    public bool IsOpen { get; private set; }
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => _devices.AsReadOnly();


    public async ValueTask OpenAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        IsOpen = true;
        foreach (var i in _devices.Values) await i.EnableAsync();
    }
    public async ValueTask CloseAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        IsOpen = false;
        foreach (var i in _devices.Values) await i.DisableAsync();
    }

    public IDevice AddDevice(IDeviceDescriptor descriptor)
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

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        await gateway.RemovePortAsync(Endpoint);
        foreach (var i in _devices.Values) await i.DisposeAsync();
    }
}
