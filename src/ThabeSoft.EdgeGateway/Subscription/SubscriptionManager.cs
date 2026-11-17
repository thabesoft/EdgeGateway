using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class SubscriptionManager : IAsyncDisposable
{
    private readonly ILifecycleKeyedCollection<SubscriptionId, IAsyncDisposableSubscription> _collection;
    private readonly ILifecycleBatchExecutor _batchExecutor;
    private readonly ISubscriptionCreator _creator;

    private readonly IEventPublisher _eventPublisher;
    private readonly PortId _portId;
    private readonly DeviceId _deviceId;


    public SubscriptionManager(
            SubscriptionManagerCreationArgs args,
            IEventPublisher eventPublisher,
            ILifecycleKeyedCollectionFactory lifecycleKeyedCollectionFactory,
            ILifecycleBatchExecutorFactory lifecycleBatchExecutorFactory)
    {
        _portId = args.Port.Id;
        _deviceId = args.Device.Id;
        _eventPublisher = eventPublisher;
        _creator = args.SubscriptionCreator;

        _collection = lifecycleKeyedCollectionFactory.Create<SubscriptionId, IAsyncDisposableSubscription>(args.DeviceState);
        _batchExecutor = lifecycleBatchExecutorFactory.Create(args.SubscriptionBatchExecutorDescriptor);

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

        _ = _eventPublisher.PublishAsync(new SubscriptionAddedEvent(_portId, _deviceId, subscription.Id));
        return (ISubscription<T>)subscription;
    }
    public async ValueTask<bool> RemoveAsync(SubscriptionId id, CancellationToken cancellationToken = default)
    {
        var result = await _collection.RemoveAsync(id, cancellationToken);
        if (!result) return false;

        _ = _eventPublisher.PublishAsync(new SubscriptionRemovedEvent(_portId, _deviceId, id));
        return true;
    }
}