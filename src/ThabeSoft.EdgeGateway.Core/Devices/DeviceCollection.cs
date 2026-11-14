using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class DeviceCollection : IAsyncDisposable
{
    private readonly ILifecycleKeyedCollection<DeviceId, Device> _devices;
    private readonly ILifecycleBatchExecutor _batchExecutor;
    private readonly Func<DeviceDescriptor, Device> _devicCreator;


    public DeviceCollection(ILifecycleStateable portLifecycleState, Func<DeviceDescriptor, Device> devicCreator)
    {
        _devicCreator = devicCreator;
        _devices = Lifecycle.KeyedCollection<DeviceId, Device>(portLifecycleState);

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

    public async ValueTask<IDevice> AddAsync(DeviceDescriptor descriptor, CancellationToken cancellationToken = default) =>
        await _devices.AddAsync(descriptor.Id, () => _devicCreator.Invoke(descriptor), cancellationToken);
    public ValueTask<bool> RemoveAsync(DeviceId deviceId, CancellationToken cancellationToken = default) =>
        _devices.RemoveAsync(deviceId, cancellationToken);
}