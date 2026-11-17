using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class PortManager : IAsyncDisposable
{
    private readonly ILifecycleKeyedCollection<PortId, Port> _collection;
    private readonly ILifecycleBatchExecutor _batchExecutor;
    private readonly IEventPublisher _eventPublisher;
    private readonly Func<PortDescriptor, Port> _portCreator;


    public PortManager(
            PortManagerCreationArgs dependencies,
            IEventPublisher eventPublisher,
            ILifecycleKeyedCollectionFactory lifecycleKeyedCollectionFactory,
            ILifecycleBatchExecutorFactory lifecycleBatchExecutorFactory)
    {
        _portCreator = dependencies.PortCreator;
        _eventPublisher = eventPublisher;

        _collection = lifecycleKeyedCollectionFactory.Create<PortId, Port>(dependencies.GatwayLifecycleState);
        _batchExecutor = lifecycleBatchExecutorFactory.Create(dependencies.PortBatchExecutorDescriptor);

        Items = new ReadOnlyDictionaryView<PortId, Port, IPort>(_collection.Items);
    }

    public IReadOnlyDictionary<PortId, IPort> Items { get; }


    public ValueTask StartAllAsync(CancellationToken cancellationToken = default) =>
        _batchExecutor.StartAsync(_collection.Values, cancellationToken);

    public ValueTask StopAllAsync(CancellationToken cancellationToken = default) =>
        _batchExecutor.StopAsync(_collection.Values, cancellationToken);
    public async ValueTask DisposeAsync()
    {
        await _batchExecutor.DisposeAsync(_collection.Values);
        await _collection.DisposeAsync();
    }

    public async ValueTask<IPort> AddAsync(PortDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        var port = await _collection.AddAsync(descriptor.Id, () => _portCreator.Invoke(descriptor), cancellationToken);
        await _eventPublisher.PublishAsync(new PortAddedEvent(descriptor.Id));

        return port;
    }
    public async ValueTask<bool> RemoveAsync(PortId id, CancellationToken cancellationToken = default)
    {
        var result = await _collection.RemoveAsync(id, cancellationToken);
        await _eventPublisher.PublishAsync(new PortRemovedEvent(id));

        return result;
    }
}