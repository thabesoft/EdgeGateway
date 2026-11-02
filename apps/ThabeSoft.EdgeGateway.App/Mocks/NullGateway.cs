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


public sealed class NullGateway : IGateway
{
    private NullGateway() { }
    public static NullGateway Instance { get; } = new();


    public LifecycleState State { get; }
    public IReadOnlyDictionary<PortId, IPort> Ports { get; init; } = new Dictionary<PortId, IPort>();


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        ValueTask.CompletedTask;
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        ValueTask.CompletedTask;

    public IPort AddPort(IPortDescriptor descriptor) =>
        NullPort.Instance;
    public ValueTask RemovePortAsync(PortId id, CancellationToken cancellationToken = default) =>
        ValueTask.CompletedTask;

    public ValueTask DisposeAsync() =>
        ValueTask.CompletedTask;
}
public sealed class NullPort : IPort
{
    private NullPort() { }
    public static NullPort Instance { get; } = new();


    public PortId Id { get; } = PortId.From(Guid.Empty.ToString());
    public ChannelAddress ChannelAddress { get; } = ChannelAddress.Serial("COM0");
    public bool IsOpen { get; }
    public IReadOnlyDictionary<DeviceId, IDevice> Devices { get; } = new Dictionary<DeviceId, IDevice>();


    public IDevice AddDevice(IDeviceDescriptor descriptor) =>
        NullDevice.Instance;
    public ValueTask CloseAsync(CancellationToken cancellationToken = default) =>
        ValueTask.CompletedTask;
    public ValueTask DisposeAsync() =>
        ValueTask.CompletedTask;

    public ValueTask OpenAsync(CancellationToken cancellationToken = default) =>
        ValueTask.CompletedTask;
    public ValueTask RemoveDeviceAsync(DeviceId id, CancellationToken cancellationToken = default) =>
        ValueTask.CompletedTask;
}
public sealed class NullDevice : IDevice
{
    private NullDevice() { }
    public static NullDevice Instance { get; } = new();


    public DeviceId Id { get; } = DeviceId.From(Guid.Empty.ToString());
    public ProtocolType ProtocolType { get; } = default;
    public bool IsEnabled { get; }
    public IReadOnlyDictionary<TagName, ITagMonitor> Monitors { get; } = new Dictionary<TagName, ITagMonitor>();


    public ValueTask DisableAsync(CancellationToken cancellationToken = default) =>
        ValueTask.CompletedTask;
    public ValueTask EnableAsync(CancellationToken cancellationToken = default) =>
        ValueTask.CompletedTask;


    public ITagMonitor<T> Register<T>(TagName tagName, ITag<T> tag, ITrigger trigger) where T : notnull =>
        NullTagMonitor<T>.Instance;
    public void Unregister(TagName tagName) { }


    public ValueTask DisposeAsync() =>
        ValueTask.CompletedTask;
}
public sealed class NullTagMonitor<T> : ITagMonitor<T>
    where T : notnull
{
    private NullTagMonitor() { }
    public static NullTagMonitor<T> Instance { get; } = new();


    public event Action<T>? Updated;
    public ITag<T> Tag { get; } = NullTag<T>.Instance;
    public TagName Name { get; } = TagName.From(typeof(T).Name);


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