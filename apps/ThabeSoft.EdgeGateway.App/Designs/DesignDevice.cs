using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.App.Designs;

public sealed class DesignDevice(DeviceDescriptor deviceDescriptor, DesignPort port) : IDevice
{
    private readonly ConcurrentDictionary<TagId, ITagMonitor> _monitors = [];


    public DeviceId Id => deviceDescriptor.DeviceId;
    public ProtocolType ProtocolType => deviceDescriptor.Protocol.Type;
    public bool IsEnabled { get; private set; }
    public IReadOnlyDictionary<TagId, ITagMonitor> TagMonitors => _monitors.AsReadOnly();


    public ValueTask EnableAsync(CancellationToken cancellationToken = default)
    {
        IsEnabled = true;
        return ValueTask.CompletedTask;
    }
    public ValueTask DisableAsync(CancellationToken cancellationToken = default)
    {
        IsEnabled = false;
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
