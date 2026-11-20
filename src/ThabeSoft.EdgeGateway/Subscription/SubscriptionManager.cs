using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.PubSub;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Primitives;
using ThabeSoft.EdgeGateway.Messages;

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
    private readonly DisposeFlag _disposeFlag = new();


    public IReadOnlyDictionary<SubscriptionId, ISubscription> Items => collection.Items;


    public ValueTask StartAllAsync(CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);
        return batchExecutor.StartBatchAsync(collection.Values, cancellationToken);
    }
    public ValueTask StopAllAsync(CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);
        return batchExecutor.StopBatchAsync(collection.Values, cancellationToken);
    }
    public async ValueTask<ISubscription<T>> AddAsync<T>(SubscriptionDescriptor<T> descriptor, CancellationToken cancellationToken = default)
        where T : notnull
    {
        _disposeFlag.ThrowIfDisposed(this);

        var subscription = await collection.AddAsync(descriptor.Id, () => subscriptionCreator.Create(descriptor), cancellationToken);
        _ = eventPublisher.PublishAsync(new SubscriptionAddedMessage(_portId, _deviceId, subscription.Id));
        return (ISubscription<T>)subscription;
    }
    public async ValueTask<bool> RemoveAsync(SubscriptionId id, CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);

        var result = await collection.RemoveAsync(id, cancellationToken);
        if (!result) return false;

        _ = eventPublisher.PublishAsync(new SubscriptionRemovedMessage(_portId, _deviceId, id));
        return true;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposeFlag.TryMarkDisposed()) return;
        await Safe.DisposeSafelyAsync(onDispose, logger);
    }
}