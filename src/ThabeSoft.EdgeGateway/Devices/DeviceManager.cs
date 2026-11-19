using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class DeviceManager(
        PortId portId,
        ILifecycleKeyedCollection<DeviceId, IDevice> collection,
        ILifecycleBatchExecutor batchExecutor,
        Func<DeviceDescriptor, Device> deviceCreator,
        IEventPublisher eventPublisher,
        ILogger<DeviceManager> logger
    ) : IAsyncDisposable
{
    public IReadOnlyDictionary<DeviceId, IDevice> Items => collection.Items;
    public Func<ValueTask>? DisposedCallback { get; init; }


    public ValueTask StartAllAsync(CancellationToken cancellationToken = default) =>
        batchExecutor.StartBatchAsync(collection.Values, cancellationToken);

    public ValueTask StopAllAsync(CancellationToken cancellationToken = default) =>
        batchExecutor.StopBatchAsync(collection.Values, cancellationToken);
    public async ValueTask DisposeAsync()
    {
        if (DisposedCallback is null) return;

        try { await DisposedCallback.Invoke(); }
        catch (Exception ex) { logger.LogError(ex, "设备管理器释放回调异常"); }
    }

    public async ValueTask<IDevice> AddAsync(DeviceDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        var device = await collection.AddAsync(descriptor.Id, () => deviceCreator.Invoke(descriptor), cancellationToken);
        _ = eventPublisher.PublishAsync(new DeviceAddedEvent(portId, device.Id));
        return device;
    }
    public async ValueTask<bool> RemoveAsync(DeviceId deviceId, CancellationToken cancellationToken = default)
    {
        var result = await collection.RemoveAsync(deviceId, cancellationToken);
        if (!result) return false;

        _ = eventPublisher.PublishAsync(new DeviceRemovedEvent(portId, deviceId));
        return true;
    }
}