using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.App.Designs;

public sealed class DesignDevice(IDeviceDescriptor deviceDescriptor) : IDevice
{
    private readonly Dictionary<TagName, ITagMonitor> _monitors = [];


    public DeviceId Id => deviceDescriptor.DeviceId;
    public ProtocolType ProtocolType => deviceDescriptor.Protocol.Type;
    public bool IsEnabled { get; private set; }
    public IReadOnlyDictionary<TagName, ITagMonitor> Monitors => _monitors.AsReadOnly();


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


    public ITagMonitor<T> Register<T>(TagName tagName, ITag<T> tag, ITrigger trigger) where T : notnull
    {
        var monitor = new DesignlTagMonitor<T>(tagName, tag);
        _monitors[tagName] = monitor;
        return monitor;
    }
    public void Unregister(TagName tagName)
    {
        _monitors.Remove(tagName);
    }

    public ValueTask DisposeAsync()
    {
        _monitors.Clear();
        return ValueTask.CompletedTask;
    }
}
