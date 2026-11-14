using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class SubscriptionCollection : IAsyncDisposable
{
    private readonly ILifecycleKeyedCollection<SubscriptionId, IAsyncDisposableSubscription> _collection;
    private readonly ILifecycleBatchExecutor _batchExecutor;
    private readonly ISubscriptionCreator _creator;


    public SubscriptionCollection(ILifecycleStateable gatwayLifecycleState, ISubscriptionCreator creator)
    {
        _creator = creator;
        _collection = Lifecycle.KeyedCollection<SubscriptionId, IAsyncDisposableSubscription>(gatwayLifecycleState);
        _batchExecutor = Lifecycle.BatchExecutor();

        Items = new ReadOnlyDictionaryView<SubscriptionId, IAsyncDisposableSubscription, ISubscription>(_collection.Items);
    }

    public IReadOnlyDictionary<SubscriptionId, ISubscription> Items { get; }


    public ValueTask StartAllAsync(CancellationToken cancellationToken = default) =>
        _batchExecutor.StartAsync(_collection.Values, cancellationToken);

    public ValueTask StopAllAsync(CancellationToken cancellationToken = default) =>
        _batchExecutor.StopAsync(_collection.Values, cancellationToken);
    public async ValueTask DisposeAsync()
    {
        await _batchExecutor.DisposeAsync(_collection.Values);
        await _collection.DisposeAsync();
    }

    public async ValueTask<ISubscription<T>> AddAsync<T>(SubscriptionDescriptor<T> descriptor, CancellationToken cancellationToken = default)
        where T : notnull
    {
        var subscription = await _collection.AddAsync(descriptor.Id, () => _creator.Create(descriptor), cancellationToken);
        return (ISubscription<T>)subscription;
    }
    public ValueTask<bool> RemoveAsync(SubscriptionId id, CancellationToken cancellationToken = default) =>
        _collection.RemoveAsync(id, cancellationToken);
}

internal interface ISubscriptionCreator
{
    Subscription<T> Create<T>(SubscriptionDescriptor<T> descriptor) where T : notnull;
}