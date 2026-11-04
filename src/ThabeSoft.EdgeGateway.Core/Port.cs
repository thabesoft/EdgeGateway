using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Devices;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway;


internal sealed class Port(
        TransportFactoryHandler transportFactoryHandler,
        DeviceFactoryHandler deviceFactoryHandler,
        PortDisposedHandler disposedHandler
    ) : ResilientLifecycleBase, IPort
{
    private readonly ITransportConnection _transportConnection = transportFactoryHandler();
    private readonly ConcurrentDictionary<DeviceId, IDevice> _devices = [];



    public ConnectionEndpoint Endpoint => _transportConnection.Endpoint;
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => _devices;
    public bool IsOpen => IsRunning;



    public ValueTask OpenAsync(CancellationToken cancellationToken = default) =>
        StartAsync(cancellationToken);
    public ValueTask CloseAsync(CancellationToken cancellationToken = default) =>
        StopAsync(cancellationToken);


    public IDevice AddDevice(IDeviceDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        var device = deviceFactoryHandler(descriptor, _transportConnection, this);
        return _devices[device.Id] = device;
    }
    public async ValueTask RemoveDeviceAsync(DeviceId id, CancellationToken cancellationToken)
    {
        if (!_devices.TryRemove(id, out var device))
            return;

        await device.DisableAsync(cancellationToken);
    }


    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        await _transportConnection.OpenAsync(cancellationToken);

        foreach (var i in _devices.Values)
            await i.EnableAsync(cancellationToken);
    }

    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        foreach (var i in _devices.Values)
            await i.DisableAsync(cancellationToken);

        await _transportConnection.CloseAsync(cancellationToken);
    }
    protected override async ValueTask OnDisposeAsync()
    {
        foreach (var i in Devices.Values) await i.DisposeAsync();
        await disposedHandler.Invoke(Endpoint);
    }
}

internal delegate ITransportConnection TransportFactoryHandler();
internal delegate IDevice DeviceFactoryHandler(IDeviceDescriptor descriptor, ITransportConnection transport, IDeviceManager manager);
internal delegate ValueTask PortDisposedHandler(ConnectionEndpoint endpoint);