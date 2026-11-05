using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class Port(ITransportConnection transportConnection, DeviceManager deviceManager) : RetryableLifecycle, IPort
{
    private readonly ConcurrentDictionary<DeviceId, IDevice> _devices = [];


    public ConnectionEndpoint Endpoint => transportConnection.Endpoint;
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => _devices;
    public bool IsOpen => IsRunning;


    public async ValueTask<IDevice> AddDeviceAsync(DeviceDescriptor descriptor)
    {
        var device = await deviceManager.RegisterAsync(descriptor, transportConnection);
        _devices[device.Id] = device;
        return device;
    }


    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        await transportConnection.StartAsync(cancellationToken);

        foreach (var i in Devices.Values)
            await i.StartAsync(cancellationToken);
    }
    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        foreach (var i in Devices.Values)
            await i.StopAsync(cancellationToken);

        await transportConnection.StopAsync(cancellationToken);
    }
    protected override async ValueTask OnDisposeAsync()
    {
        foreach (var i in _devices.Values)
            await i.DisposeAsync();
    }
}