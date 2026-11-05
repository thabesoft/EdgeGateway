using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class PortManageer(TransportManager transportManager, DeviceManager deviceManager)
{
    private readonly ConcurrentDictionary<ConnectionEndpoint, IPort> _entries = [];

    public async ValueTask<IPort> RegisterAsync(ITransportDescriptor transportDescriptor)
    {
        var transportConnection = await transportManager.RegisterTransportAsync(transportDescriptor);
        var port = new Port(transportConnection, deviceManager);
        var entry = new Entry(port, transportConnection, this);

        if (_entries.TryAdd(transportDescriptor.Endpoint, entry)) return entry;

        await entry.DisposeAsync();
        throw new InvalidDataException();
    }

    private sealed class Entry(Port port, ITransportConnection transportConnection, PortManageer portManageer) : IPort
    {
        public async ValueTask DisposeAsync()
        {
            await port.DisposeAsync();
            await transportConnection.DisposeAsync();

            portManageer._entries.TryRemove(Endpoint, out _);
        }


        public ConnectionEndpoint Endpoint => port.Endpoint;
        public LifecycleState State => port.State;
        public IReadOnlyDictionary<DeviceId, IDevice> Devices => port.Devices;


        public ValueTask<IDevice> AddDeviceAsync(DeviceDescriptor descriptor) =>
            port.AddDeviceAsync(descriptor);
        public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
            port.StartAsync(cancellationToken);
        public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
            port.StopAsync(cancellationToken);
    }
}
