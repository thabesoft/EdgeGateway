using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class PortManageer(
        TransportManager transportManager,
        DeviceManager deviceManager,
        ILoggerFactory loggerFactory
    ) : IAsyncDisposable
{
    private readonly ILogger<PortManageer> _logger = loggerFactory.CreateLogger<PortManageer>();
    private readonly ConcurrentDictionary<ConnectionEndpoint, IPort> _entries = [];


    public IPort Register(ITransportDescriptor transportDescriptor)
    {
        var transportConnection = transportManager.Register(transportDescriptor);
        var port = new Port(transportConnection, deviceManager);
        var entry = new Entry(port, transportConnection, this, _logger);

        if (_entries.TryAdd(transportDescriptor.Endpoint, entry)) return entry;

        entry.Dispose();
        throw new InvalidDataException();
    }
    public async ValueTask DisposeAsync()
    {
        foreach (var i in _entries.Values)
            await i.DisposeAsync();

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("端口管理器已释放");
    }


    private sealed class Entry(
            Port port,
            ITransportConnection transportConnection,
            PortManageer portManageer,
            ILogger logger
        ) : IPort, IDisposable
    {
        private int _disposed;

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

            await port.DisposeAsync();
            await transportConnection.DisposeAsync();

            portManageer._entries.TryRemove(Endpoint, out _);

            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("端口 [{Endpoint}] 释放", Endpoint);
        }
        public async void Dispose()
        {
            await DisposeAsync();
        }


        public ConnectionEndpoint Endpoint => port.Endpoint;
        public LifecycleState State => port.State;
        public IReadOnlyDictionary<DeviceId, IDevice> Devices => port.Devices;


        public IDevice AddDevice(DeviceDescriptor descriptor)
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("端口 [{Endpoint}] 添加设备 [{DeviceId}]", Endpoint, descriptor.Id);

            return port.AddDevice(descriptor);
        }
        public ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("端口 [{Endpoint}] 启动", Endpoint);

            return port.StartAsync(cancellationToken);
        }
        public ValueTask StopAsync(CancellationToken cancellationToken = default)
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("端口 [{Endpoint}] 停止", Endpoint);

            return port.StopAsync(cancellationToken);
        }
    }
}
