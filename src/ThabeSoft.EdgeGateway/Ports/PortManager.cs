using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Messages;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.Primitives;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.PubSub;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class PortManager(PortManagerDependencies dependencies) : IAsyncDisposable
{
    public IReadOnlyDictionary<PortId, IPort> Items => dependencies.Collection.Items;
    public Func<ValueTask>? DisposedCallback { get; init; }


    public ValueTask StartAllAsync(CancellationToken cancellationToken = default) =>
        dependencies.BatchExecutor.StartBatchAsync(dependencies.Collection.Values, cancellationToken);

    public ValueTask StopAllAsync(CancellationToken cancellationToken = default) =>
        dependencies.BatchExecutor.StopBatchAsync(dependencies.Collection.Values, cancellationToken);
    public ValueTask DisposeAsync() =>
        Safe.DisposeSafelyAsync(dependencies.OnDispose, dependencies.Logger);

    public async ValueTask<IPort> AddAsync(PortDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        var port = await dependencies.Collection.AddAsync(descriptor.Id, () => dependencies.PortCreator.Invoke(descriptor), cancellationToken);
        _ = dependencies.EventPublisher.PublishAsync(new PortAddedMessage(descriptor.Id), cancellationToken);

        return port;
    }
    public async ValueTask<bool> RemoveAsync(PortId id, CancellationToken cancellationToken = default)
    {
        if (await dependencies.Collection.RemoveAsync(id, cancellationToken))
        {
            _ = dependencies.EventPublisher.PublishAsync(new PortRemovedMessage(id), cancellationToken);
            return true;
        }

        return false;
    }
}

internal sealed class PortManagerDependencies
{
    public required ILifecycleKeyedCollection<PortId, IPort> Collection { get; init => field = value.Required(); }
    public required ILifecycleBatchExecutor BatchExecutor { get; init => field = value.Required(); }

    // Services
    public required Func<ValueTask> OnDispose { get; init => field = value.Required(); }
    public required IEventPublisher EventPublisher { get; init => field = value.Required(); }
    public required ILogger<PortManager> Logger { get; init => field = value.Required(); }
    public required Func<PortDescriptor, Port> PortCreator { get; init => field = value.Required(); }
}