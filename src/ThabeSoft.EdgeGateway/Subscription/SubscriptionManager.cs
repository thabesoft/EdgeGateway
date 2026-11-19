using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class SubscriptionManager(
        PortId portId,
        DeviceId deviceId,
        ILifecycleKeyedCollection<SubscriptionId, ISubscription> collection,
        ILifecycleBatchExecutor batchExecutor,
        ISubscriptionCreator subscriptionCreator,
        IEventPublisher eventPublisher,
        ILogger<SubscriptionManager> logger
    ) : IAsyncDisposable
{
    private readonly PortId _portId = portId;
    private readonly DeviceId _deviceId = deviceId;


    public IReadOnlyDictionary<SubscriptionId, ISubscription> Items => collection.Items;
    public Func<ValueTask>? DisposedCallback { get; init; }


    public ValueTask StartAllAsync(CancellationToken cancellationToken = default) =>
        batchExecutor.StartAsync(collection.Values, cancellationToken);

    public ValueTask StopAllAsync(CancellationToken cancellationToken = default) =>
        batchExecutor.StopAsync(collection.Values, cancellationToken);
    public async ValueTask DisposeAsync()
    {
        if (DisposedCallback is null) return;

        try { await DisposedCallback.Invoke(); }
        catch (Exception ex) { logger.LogError(ex, "订阅管理器释放回调异常"); }
    }

    public async ValueTask<ISubscription<T>> AddAsync<T>(SubscriptionDescriptor<T> descriptor, CancellationToken cancellationToken = default)
        where T : notnull
    {
        var subscription = await collection.AddAsync(descriptor.Id, () => subscriptionCreator.Create(descriptor), cancellationToken);

        _ = eventPublisher.PublishAsync(new SubscriptionAddedEvent(_portId, _deviceId, subscription.Id));
        return (ISubscription<T>)subscription;
    }
    public async ValueTask<bool> RemoveAsync(SubscriptionId id, CancellationToken cancellationToken = default)
    {
        var result = await collection.RemoveAsync(id, cancellationToken);
        if (!result) return false;

        _ = eventPublisher.PublishAsync(new SubscriptionRemovedEvent(_portId, _deviceId, id));
        return true;
    }
}