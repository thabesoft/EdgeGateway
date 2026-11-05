using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.App.Designs;

public sealed class DesignDevice(DeviceDescriptor deviceDescriptor, DesignPort port) : IDevice
{
    private readonly ConcurrentDictionary<TagId, ITagMonitor> _monitors = [];


    public DeviceId Id => deviceDescriptor.DeviceId;
    public ProtocolType ProtocolType => deviceDescriptor.Protocol.Type;
    public IReadOnlyDictionary<TagId, ITagMonitor> TagMonitors => _monitors.AsReadOnly();
    public LifecycleState State { get; private set; }


    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        State = LifecycleState.Running;
        return ValueTask.CompletedTask;
    }
    public ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        State = LifecycleState.Stopping;
        return ValueTask.CompletedTask;
    }


    public ITagMonitor<T> Register<T>(TagDescriptor<T> descriptor, ITrigger trigger) where T : notnull
    {
        var monitor = new DesignTagMonitor<T>(descriptor);
        _monitors[descriptor.Id] = monitor;
        return monitor;
    }
    public void Unregister(TagId id)
    {
        _monitors.TryRemove(id, out _);
    }

    public async ValueTask DisposeAsync()
    {
        _monitors.Clear();
        await port.RemoveDeviceAsync(Id);
    }
}
