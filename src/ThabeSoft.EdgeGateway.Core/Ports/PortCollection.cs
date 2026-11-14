using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class PortCollection : IAsyncDisposable
{
    private readonly ILifecycleKeyedCollection<ConnectionEndpoint, Port> _collection;
    private readonly ILifecycleBatchExecutor _batchExecutor;
    private readonly Func<PortDescriptor, Port> _creator;


    public PortCollection(ILifecycleStateable gatwayLifecycleState, Func<PortDescriptor, Port> creator)
    {
        _creator = creator;
        _collection = Lifecycle.KeyedCollection<ConnectionEndpoint, Port>(gatwayLifecycleState);
        _batchExecutor = Lifecycle.BatchExecutor();

        Items = new ReadOnlyDictionaryView<ConnectionEndpoint, Port, IPort>(_collection.Items);
    }

    public IReadOnlyDictionary<ConnectionEndpoint, IPort> Items { get; }


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
        await _collection.AddAsync(descriptor.Transport.Endpoint, () => _creator.Invoke(descriptor), cancellationToken);
    public ValueTask<bool> RemoveAsync(ConnectionEndpoint endpoint, CancellationToken cancellationToken = default) =>
        _collection.RemoveAsync(endpoint, cancellationToken);
}