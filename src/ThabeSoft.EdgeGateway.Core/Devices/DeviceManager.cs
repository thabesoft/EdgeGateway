using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class DeviceManager(
        SubscriptionManager subscriptionManager,
        ILoggerFactory loggerFactory
    ) : IAsyncDisposable
{
    private readonly ILogger<DeviceManager> _logger = loggerFactory.CreateLogger<DeviceManager>();
    private readonly ConcurrentDictionary<DeviceId, IDevice> _entries = [];


    public IDevice Register(DeviceDescriptor descriptor, IByteDuplex byteDuplex)
    {
        var device = new Device(descriptor, subscriptionManager, byteDuplex, loggerFactory.CreateLogger<Device>());

        var entry = new Entry(device, this, _logger);
        if (_entries.TryAdd(descriptor.DeviceId, entry)) return entry;

        entry.Dispose();
        throw new InvalidOperationException("创建是啊比啊");
    }
    public async ValueTask DisposeAsync()
    {
        foreach (var i in _entries.Values)
            await i.DisposeAsync();

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("设备管理器已释放");
    }


    private sealed class Entry(IDevice device, DeviceManager manager, ILogger logger) : IDevice, IDisposable
    {
        private int _disposed;

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

            await device.DisposeAsync();
            manager._entries.TryRemove(Id, out _);

            if (logger.IsEnabled(LogLevel.Trace))
                logger.LogTrace("设备 [{Did}] 释放", Id);
        }
        public async void Dispose()
        {
            await DisposeAsync();
        }


        public DeviceId Id => device.Id;
        public ProtocolType ProtocolType => device.ProtocolType;
        public LifecycleState State => device.State;
        public IReadOnlyDictionary<SubscriptionId, ISubscription> Subscriptions => device.Subscriptions;


        public ISubscription<T> AddSubscription<T>(SubscriptionDescriptor<T> subscriptionDescriptor) where T : notnull
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("设备 [{Did}] 添加订阅 [{Sid}]", Id, subscriptionDescriptor.Id);

            return device.AddSubscription(subscriptionDescriptor);
        }
        public ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("设备 [{Did}] 启动", Id);

            return device.StartAsync(cancellationToken);
        }
        public ValueTask StopAsync(CancellationToken cancellationToken = default)
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("设备 [{Did}] 停止", Id);

            return device.StopAsync(cancellationToken);
        }
    }
}