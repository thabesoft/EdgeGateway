using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class DeviceManager(SubscriptionManager subscriptionManager, ILoggerFactory loggerFactory)
{
    private readonly ConcurrentDictionary<DeviceId, IDevice> _entries = [];

    public async ValueTask<IDevice> RegisterAsync(DeviceDescriptor descriptor, IByteDuplex byteDuplex)
    {
        var device = new Device(descriptor, subscriptionManager, byteDuplex, loggerFactory.CreateLogger<Device>());

        var entry = new Entry(device, this);
        if (_entries.TryAdd(descriptor.DeviceId, entry)) return entry;

        await entry.DisposeAsync();
        throw new InvalidOperationException("创建是啊比啊");
    }

    private sealed class Entry(IDevice device, DeviceManager manager) : IDevice
    {
        public async ValueTask DisposeAsync()
        {
            await device.DisposeAsync();

            manager._entries.TryRemove(Id, out _);
        }


        public DeviceId Id => device.Id;
        public ProtocolType ProtocolType => device.ProtocolType;
        public LifecycleState State => device.State;
        public IReadOnlyCollection<ISubscription> Subscriptions => device.Subscriptions;

        public ValueTask<ISubscription<T>> AddSubscriptionAsync<T>(SubscriptionDescriptor<T> subscriptionDescriptor) where T : notnull =>
            device.AddSubscriptionAsync(subscriptionDescriptor);
        public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
            device.StartAsync(cancellationToken);
        public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
            device.StopAsync(cancellationToken);
    }
}