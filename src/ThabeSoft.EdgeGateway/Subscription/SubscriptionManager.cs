using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Primitives;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class SubscriptionManager(
        PortId portId,
        DeviceId deviceId,
        ILifecycleKeyedCollection<SubscriptionId, ISubscription> collection,
        ILifecycleBatchExecutor batchExecutor,
        ISubscriptionCreator subscriptionCreator,
        IEventPublisher eventPublisher,
        ILogger<SubscriptionManager> logger,
        Func<ValueTask> onDispose
    ) : IAsyncDisposable
{
    private readonly PortId _portId = portId;
    private readonly DeviceId _deviceId = deviceId;
    private readonly AsyncDisposableToken _disposeToken = new(onDispose, logger);


    public IReadOnlyDictionary<SubscriptionId, ISubscription> Items => collection.Items;


    public ValueTask StartAllAsync(CancellationToken cancellationToken = default)
    {
        _disposeToken.ThrowIfDisposed(this);
        return batchExecutor.StartBatchAsync(collection.Values, cancellationToken);
    }
    public ValueTask StopAllAsync(CancellationToken cancellationToken = default)
    {
        _disposeToken.ThrowIfDisposed(this);
        return batchExecutor.StopBatchAsync(collection.Values, cancellationToken);
    }
    public ValueTask TerminateAllAsync(CancellationToken cancellationToken = default)
    {
        _disposeToken.ThrowIfDisposed(this);
        return batchExecutor.TerminateBatchAsync(collection.Values, cancellationToken);
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


    public ValueTask DisposeAsync() => _disposeToken.DisposeAsync();
}