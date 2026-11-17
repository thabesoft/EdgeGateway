using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Utilities;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class DeviceManager : IAsyncDisposable
{
    private readonly ILifecycleKeyedCollection<DeviceId, Device> _collection;
    private readonly ILifecycleBatchExecutor _batchExecutor;
    private readonly PortId _portId;
    private readonly IEventPublisher _eventPublisher;
    private readonly Func<DeviceDescriptor, Device> _devicCreator;


    public DeviceManager(
            DeviceManagerDependencies dependencies,
            IEventPublisher eventPublisher,
            ILifecycleKeyedCollectionFactory lifecycleKeyedCollectionFactory,
            ILifecycleBatchExecutorFactory lifecycleBatchExecutorFactory)
    {
        _devicCreator = dependencies.DevicCreator;
        _portId = dependencies.Port.Id;
        _eventPublisher = eventPublisher;

        _collection = lifecycleKeyedCollectionFactory.Create<DeviceId, Device>(dependencies.PortLifecycleState);
        _batchExecutor = lifecycleBatchExecutorFactory.Create(dependencies.DeviceBatchExecutorDescriptor);

        Items = new ReadOnlyDictionaryView<DeviceId, Device, IDevice>(_collection.Items);
    }

    public IReadOnlyDictionary<DeviceId, IDevice> Items { get; }


    public ValueTask StartAllAsync(CancellationToken cancellationToken = default) =>
        _batchExecutor.StartAsync(_collection.Values, cancellationToken);

    public ValueTask StopAllAsync(CancellationToken cancellationToken = default) =>
        _batchExecutor.StopAsync(_collection.Values, cancellationToken);
    public async ValueTask DisposeAsync()
    {
        await _batchExecutor.DisposeAsync(_collection.Values);
        await _collection.DisposeAsync();
    }

    public async ValueTask<IDevice> AddAsync(DeviceDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        var device = await _collection.AddAsync(descriptor.Id, () => _devicCreator.Invoke(descriptor), cancellationToken);
        _ = _eventPublisher.PublishAsync(new DeviceAddedEvent(_portId, device.Id));
        return device;
    }
    public async ValueTask<bool> RemoveAsync(DeviceId deviceId, CancellationToken cancellationToken = default)
    {
        var result = await _collection.RemoveAsync(deviceId, cancellationToken);
        if (!result) return false;

        _ = _eventPublisher.PublishAsync(new DeviceRemovedEvent(_portId, deviceId));
        return true;
    }
}


internal readonly struct DeviceManagerDependencies
{
    // Port
    public required PortDescriptor Port { get; init => field = value.Required(); }
    public required ILifecycleStateable PortLifecycleState { get; init => field = value.Required(); }

    // Device
    public required Func<DeviceDescriptor, Device> DevicCreator { get; init => field = value.Required(); }
    public required LifecycleBatchExecutorDescriptor DeviceBatchExecutorDescriptor { get; init => field = value.Required(); }
}


internal sealed class DeviceManagerFactory(
        IEventPublisher eventPublisher,
        ILifecycleKeyedCollectionFactory lifecycleKeyedCollectionFactory,
        ILifecycleBatchExecutorFactory lifecycleBatchExecutorFactory
    )
{
    public DeviceManager Create(DeviceManagerDependencies dependencies)
    {
        return new(
            dependencies: dependencies,
            eventPublisher: eventPublisher,
            lifecycleKeyedCollectionFactory: lifecycleKeyedCollectionFactory,
            lifecycleBatchExecutorFactory: lifecycleBatchExecutorFactory);
    }
}