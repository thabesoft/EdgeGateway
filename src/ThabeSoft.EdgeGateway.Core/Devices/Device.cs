using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class Device(
        DeviceDescriptor deviceDescriptor,
        SubscriptionManager subscriptionManager,
        IByteDuplex byteDuplex,
        ILogger<Device> logger
    ) : RetryableLifecycle(logger), IDevice
{
    private readonly ConcurrentDictionary<SubscriptionId, ISubscription> _subscriptions = [];


    public DeviceId Id => deviceDescriptor.DeviceId;
    public ProtocolType ProtocolType => deviceDescriptor.Protocol.Type;
    public IReadOnlyDictionary<SubscriptionId, ISubscription> Subscriptions => _subscriptions;


    public ISubscription<T> AddSubscription<T>(SubscriptionDescriptor<T> subscriptionDescriptor) where T : notnull
    {
        var subscription =  subscriptionManager.Register(subscriptionDescriptor, deviceDescriptor.Protocol, byteDuplex);
        _subscriptions[subscriptionDescriptor.Id] = subscription;
        return subscription;
    }

    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        foreach (var i in Subscriptions.Values)
            await i.StartAsync(cancellationToken);
    }
    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        foreach (var i in Subscriptions.Values)
            await i.StopAsync(cancellationToken);
    }
    protected override async ValueTask OnDisposeAsync()
    {
        foreach (var i in Subscriptions.Values)
            await i.DisposeAsync();
    }
}