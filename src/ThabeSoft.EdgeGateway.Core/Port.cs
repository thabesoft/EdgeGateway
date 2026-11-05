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
    ) : RetryableLifecycle, IPort
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


    public IDevice AddDevice(DeviceDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        var device = deviceFactoryHandler(descriptor, _transportConnection, this);
        return _devices[device.Id] = device;
    }
    public async ValueTask RemoveDeviceAsync(DeviceId id, CancellationToken cancellationToken)
    {
        if (!_devices.TryRemove(id, out var device))
            return;

        await device.DisposeAsync();
    }


    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        await _transportConnection.StartAsync(cancellationToken);

        foreach (var i in _devices.Values)
            await i.StartAsync(cancellationToken);
    }

    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        foreach (var i in _devices.Values)
            await i.StopAsync(cancellationToken);

        await _transportConnection.StopAsync(cancellationToken);
    }
    protected override async ValueTask OnDisposeAsync()
    {
        foreach (var i in Devices.Values) await i.DisposeAsync();
        await disposedHandler.Invoke(Endpoint);
    }
}

internal delegate ITransportConnection TransportFactoryHandler();
internal delegate IDevice DeviceFactoryHandler(DeviceDescriptor descriptor, ITransportConnection transport, IDeviceManager manager);
internal delegate ValueTask PortDisposedHandler(ConnectionEndpoint endpoint);