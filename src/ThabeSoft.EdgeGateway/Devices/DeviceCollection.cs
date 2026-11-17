using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Utilities;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class DeviceCollection : IAsyncDisposable
{
    private readonly ILifecycleKeyedCollection<DeviceId, Device> _devices;
    private readonly ILifecycleBatchExecutor _batchExecutor;
    private readonly PortId _portId;
    private readonly IEventPublisher _eventPublisher;
    private readonly Func<DeviceDescriptor, Device> _devicCreator;


    public DeviceCollection(DeviceCollectionDependencies dependencies)
    {
        _devicCreator = dependencies.DevicCreator;
        _portId = dependencies.PortId;
        _eventPublisher = dependencies.EventPublisher;

        _devices = Lifecycle.KeyedCollection<DeviceId, Device>(dependencies.PortLifecycleState);
        _batchExecutor = Lifecycle.BatchExecutor();

        Items = new ReadOnlyDictionaryView<DeviceId, Device, IDevice>(_devices.Items);
    }

    public IReadOnlyDictionary<DeviceId, IDevice> Items { get; }


    public ValueTask StartAllAsync(CancellationToken cancellationToken = default) =>
        _batchExecutor.StartAsync(_devices.Values, cancellationToken);

    public ValueTask StopAllAsync(CancellationToken cancellationToken = default) =>
        _batchExecutor.StopAsync(_devices.Values, cancellationToken);
    public async ValueTask DisposeAsync()
    {
        await _batchExecutor.DisposeAsync(_devices.Values);
        await _devices.DisposeAsync();
    }

    public async ValueTask<IDevice> AddAsync(DeviceDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        var device = await _devices.AddAsync(descriptor.Id, () => _devicCreator.Invoke(descriptor), cancellationToken);
        _ = _eventPublisher.PublishAsync(new DeviceAddedEvent(_portId, device.Id));
        return device;
    }
    public async ValueTask<bool> RemoveAsync(DeviceId deviceId, CancellationToken cancellationToken = default)
    {
        var result = await _devices.RemoveAsync(deviceId, cancellationToken);
        if (!result) return false;

        _ = _eventPublisher.PublishAsync(new DeviceRemovedEvent(_portId, deviceId));
        return true;
    }
}


internal readonly struct DeviceCollectionDependencies
{
    public required ILifecycleStateable PortLifecycleState { get; init => field = value.Required(); }
    public required PortId PortId { get; init => field = value.Required(); }
    public required IEventPublisher EventPublisher { get; init => field = value.Required(); }
    public required Func<DeviceDescriptor, Device> DevicCreator { get; init => field = value.Required(); }
}