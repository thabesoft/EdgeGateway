using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.App.Mocks;


public sealed class DesignGateway : IGateway
{
    private readonly Dictionary<PortId, IPort> _ports = [];

    public LifecycleState State { get; private set; }
    public IReadOnlyDictionary<PortId, IPort> Ports => _ports.AsReadOnly();


    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        State = LifecycleState.Running;
        return ValueTask.CompletedTask;
    }
    public ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        State = LifecycleState.Stopped;
        return ValueTask.CompletedTask;
    }

    public IPort AddPort(IPortDescriptor descriptor)
    {
        var port = new DesignPort(descriptor);
        _ports[port.Id] = port;

        return port;
    }
    public ValueTask RemovePortAsync(PortId id, CancellationToken cancellationToken = default)
    {
        _ports.Remove(id);
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _ports.Clear();
        return ValueTask.CompletedTask;
    }
}
public sealed class DesignPort(IPortDescriptor portDescriptor) : IPort
{
    private readonly Dictionary<DeviceId, IDevice> _devices = [];

    public PortId Id { get; } = PortId.Create();
    public ChannelAddress ChannelAddress => portDescriptor.ChannelDescriptor.Address;
    public bool IsOpen { get; private set; }
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => _devices.AsReadOnly();


    public ValueTask OpenAsync(CancellationToken cancellationToken = default)
    {
        IsOpen = true;
        return ValueTask.CompletedTask;
    }
    public ValueTask CloseAsync(CancellationToken cancellationToken = default)
    {
        IsOpen = false;
        return ValueTask.CompletedTask;
    }

    public IDevice AddDevice(IDeviceDescriptor descriptor)
    {
        var device = new DesignDevice(descriptor);
        _devices[device.Id] = device;
        return device;
    }
    public ValueTask RemoveDeviceAsync(DeviceId id, CancellationToken cancellationToken = default)
    {
        _devices.Remove(id);
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _devices.Clear();
        return ValueTask.CompletedTask;
    }
}
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
public sealed class DesignlTagMonitor<T>(TagName tagName, ITag<T> tag) : ITagMonitor<T>
    where T : notnull
{
    public event Action<T>? Updated;
    public TagName Name => tagName;
    public ITag<T> Tag => tag;


    public T? Value { get; } = default;
    public DateTimeOffset UpdateTime { get; } = DateTimeOffset.MinValue;
    public TagValueState State { get; } = TagValueState.Bad;
}


public sealed class NullTag<T> : ITag<T>
    where T : notnull
{
    private NullTag() { }
    public static NullTag<T> Instance { get; } = new();


    public ICodec<T> Codec { get; } = NullCodec<T>.Instance;
    public IDataAddress Address { get; } = NullDataAddress.Instance;
}
public sealed class NullCodec<T> : ICodec<T>
    where T : notnull
{
    private NullCodec() { }
    public static NullCodec<T> Instance { get; } = new();


    public T Decode(ReadOnlySpan<byte> raw) => default!;
    public int Encode(T value, Span<byte> buffer) => 0;
}
public sealed class NullDataAddress : IDataAddress
{
    private NullDataAddress() { }
    public static NullDataAddress Instance { get; } = new();

    public bool Equals(IDataAddress? other) => other is NullDataAddress;
}