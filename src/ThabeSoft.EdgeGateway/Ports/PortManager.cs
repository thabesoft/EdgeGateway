using Microsoft.Extensions.Logging;
using ThabeSoft.PubSub;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Messages;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class PortManager(
        ILifecycleKeyedCollection<PortId, IPort> collection,
        ILifecycleBatchExecutor batchExecutor,
        Func<PortDescriptor, Port> portCreator,
        IEventPublisher eventPublisher,
        ILogger<PortManager> logger
    ) : IAsyncDisposable
{
    public IReadOnlyDictionary<PortId, IPort> Items => collection.Items;
    public Func<ValueTask>? DisposedCallback { get; init; }


    public ValueTask StartAllAsync(CancellationToken cancellationToken = default) =>
        batchExecutor.StartBatchAsync(collection.Values, cancellationToken);

    public ValueTask StopAllAsync(CancellationToken cancellationToken = default) =>
        batchExecutor.StopBatchAsync(collection.Values, cancellationToken);
    public async ValueTask DisposeAsync()
    {
        if (DisposedCallback is null) return;

        try { await DisposedCallback.Invoke(); }
        catch (Exception ex) { logger.LogError(ex, "设备管理器释放回调异常"); }
    }

    public async ValueTask<IPort> AddAsync(PortDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        var port = await collection.AddAsync(descriptor.Id, () => portCreator.Invoke(descriptor), cancellationToken);
        await eventPublisher.PublishAsync(new PortAddedMessage(descriptor.Id));

        return port;
    }
    public async ValueTask<bool> RemoveAsync(PortId id, CancellationToken cancellationToken = default)
    {
        var result = await collection.RemoveAsync(id, cancellationToken);
        await eventPublisher.PublishAsync(new PortRemovedMessage(id));

        return result;
    }
}