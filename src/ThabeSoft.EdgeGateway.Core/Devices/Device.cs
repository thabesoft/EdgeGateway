using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class Device(
        DeviceDescriptor descriptor,
        SubscriptionManager subscriptionManager,
        IByteDuplex byteDuplex,
        ILogger<Device> logger
    ) : RetryableLifecycle(logger), IDevice
{
    private readonly ConcurrentDictionary<ISubscription, byte> _subscriptions = [];


    public DeviceId Id => descriptor.DeviceId;
    public ProtocolType ProtocolType => descriptor.Protocol.Type;
    public IReadOnlyCollection<ISubscription> Subscriptions => [.. _subscriptions.Keys];


    public async ValueTask<ISubscription<T>> AddSubscriptionAsync<T>(SubscriptionDescriptor<T> subscriptionDescriptor) where T : notnull
    {
        var subscription = await subscriptionManager.RegisterAsync(subscriptionDescriptor, descriptor.Protocol, byteDuplex);
        _subscriptions[subscription] = 0;
        return subscription;
    }

    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        foreach (var i in Subscriptions)
            await i.StartAsync(cancellationToken);
    }
    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        foreach (var i in Subscriptions)
            await i.StopAsync(cancellationToken);
    }
    protected override async ValueTask OnDisposeAsync()
    {
        foreach (var i in Subscriptions)
            await i.DisposeAsync();
    }
}