using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class PortCollection : IAsyncDisposable
{
    private readonly ILifecycleKeyedCollection<PortId, Port> _collection;
    private readonly ILifecycleBatchExecutor _batchExecutor;
    private readonly Func<PortDescriptor, Port> _creator;


    public PortCollection(ILifecycleStateable gatwayLifecycleState, Func<PortDescriptor, Port> creator)
    {
        _creator = creator;
        _collection = Lifecycle.KeyedCollection<PortId, Port>(gatwayLifecycleState);
        _batchExecutor = Lifecycle.BatchExecutor();

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

    public async ValueTask<IPort> AddAsync(PortDescriptor descriptor, CancellationToken cancellationToken = default) =>
        await _collection.AddAsync(descriptor.Id, () => _creator.Invoke(descriptor), cancellationToken);
    public ValueTask<bool> RemoveAsync(PortId id, CancellationToken cancellationToken = default) =>
        _collection.RemoveAsync(id, cancellationToken);
}