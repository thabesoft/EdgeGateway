using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Utilities;
using ThabeSoft.Guard;

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
        SubscriptionManagerDependencies dependencies,
        IEventPublisher eventPublisher,
        ILifecycleKeyedCollectionFactory lifecycleKeyedCollectionFactory,
        ILifecycleBatchExecutorFactory lifecycleBatchExecutorFactory)
    {
        _portId = dependencies.PortId;
        _deviceId = dependencies.DeviceId;
        _eventPublisher = eventPublisher;
        _creator = dependencies.SubscriptionCreator;

        _collection = lifecycleKeyedCollectionFactory.Create<SubscriptionId, IAsyncDisposableSubscription>(dependencies.DeviceState);
        _batchExecutor = lifecycleBatchExecutorFactory.Create(dependencies.SubscriptionBatchExecutorDescriptor);

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

internal interface ISubscriptionCreator
{
    Subscription<T> Create<T>(SubscriptionDescriptor<T> descriptor) where T : notnull;
}

internal readonly struct SubscriptionManagerDependencies
{
    public required PortId PortId { get; init => field = value.Required(); }
    public required DeviceId DeviceId { get; init => field = value.Required(); }
    public required ILifecycleStateable DeviceState { get; init => field = value.Required(); }

    public required ISubscriptionCreator SubscriptionCreator { get; init => field = value.Required(); }
    public required LifecycleBatchExecutorDescriptor SubscriptionBatchExecutorDescriptor { get; init => field = value.Required(); }
}


internal sealed class SubscriptionManagerFactory(
        IEventPublisher eventPublisher,
        ILifecycleBatchExecutorFactory lifecycleBatchExecutorFactory,
        ILifecycleKeyedCollectionFactory lifecycleKeyedCollectionFactory
    )
{
    public SubscriptionManager Create(SubscriptionManagerDependencies dependencies)
    {
        return new(
            dependencies: dependencies,
            eventPublisher: eventPublisher,
            lifecycleKeyedCollectionFactory: lifecycleKeyedCollectionFactory,
            lifecycleBatchExecutorFactory: lifecycleBatchExecutorFactory);
    }
}