using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Messages;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.Primitives;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.PubSub;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class DeviceManager(DeviceManagerDependencies dependencies) : IAsyncDisposable
{
    private DisposeFlag _disposeFlag = new();
    public IReadOnlyDictionary<DeviceId, IDevice> Items => dependencies.Collection.Items;


    public ValueTask StartAllAsync(CancellationToken cancellationToken = default) =>
        dependencies.BatchExecutor.StartBatchAsync(dependencies.Collection.Values, cancellationToken);
    public ValueTask StopAllAsync(CancellationToken cancellationToken = default) =>
        dependencies.BatchExecutor.StopBatchAsync(dependencies.Collection.Values, cancellationToken);

    public async ValueTask<IDevice> AddAsync(DeviceDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        var device = await dependencies.Collection.AddAsync(descriptor.Id, () => dependencies.DeviceCreator.Invoke(descriptor), cancellationToken);
        _ = dependencies.EventPublisher.PublishAsync(new DeviceAddedMessage(dependencies.Port.Id, device.Id), cancellationToken);
        return device;
    }
    public async ValueTask<bool> RemoveAsync(DeviceId deviceId, CancellationToken cancellationToken = default)
    {
        if (await dependencies.Collection.RemoveAsync(deviceId, cancellationToken))
        {
            _ = dependencies.EventPublisher.PublishAsync(new DeviceRemovedMessage(dependencies.Port.Id, deviceId), cancellationToken);
            return true;
        }
        return false;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposeFlag.TryMarkDisposed()) return;
        await Safe.DisposeSafelyAsync(dependencies.OnDispose, dependencies.Logger);
    }
}


internal sealed class DeviceManagerDependencies
{
    // 端口
    public required PortDescriptor Port { get; init => field = value.Required(); }

    // Device
    public required ILifecycleKeyedCollection<DeviceId, IDevice> Collection { get; init => field = value.Required(); }
    public required ILifecycleBatchExecutor BatchExecutor { get; init => field = value.Required(); }

    // Services
    public required Func<ValueTask> OnDispose { get; init => field = value.Required(); }
    public required IEventPublisher EventPublisher { get; init => field = value.Required(); }
    public required ILogger<DeviceManager> Logger { get; init => field = value.Required(); }
    public required Func<DeviceDescriptor, Device> DeviceCreator { get; init => field = value.Required(); }
}