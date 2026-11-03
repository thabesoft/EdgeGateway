using Microsoft.Extensions.Logging;
using System.Diagnostics;
using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.App.Designs;


public sealed class DesignGateway(ILogger<DesignGateway> logger) : IGateway
{
    private readonly Dictionary<ChannelAddress, IPort> _ports = [];

    public LifecycleState State { get; private set; }
    public IReadOnlyDictionary<ChannelAddress, IPort> Ports => _ports.AsReadOnly();


    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        State = LifecycleState.Running;
        logger.LogInformation("网关已启动");
        Debug.Write("😫😫😫😫😫😫😫😫😫");

        return ValueTask.CompletedTask;
    }
    public ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        State = LifecycleState.Stopped;
        logger.LogInformation("网关已停止");
        Debug.Write("😫😫😫😫😫😫😫😫😫");

        return ValueTask.CompletedTask;
    }

    public IPort AddPort(IPortDescriptor descriptor)
    {
        var port = new DesignPort(descriptor);
        _ports[port.ChannelAddress] = port;

        logger.LogInformation("网关添加端口, 通道地址:{Addr}", descriptor.ChannelDescriptor.Address);
        return port;
    }
    public ValueTask RemovePortAsync(ChannelAddress channelAddress, CancellationToken cancellationToken = default)
    {
        _ports.Remove(channelAddress);
        logger.LogInformation("网关删除端口, 通道地址:{Addr}", channelAddress);

        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _ports.Clear();
        logger.LogInformation("网关释放");
        Debug.Write("😫😫😫😫😫😫😫😫😫");

        return ValueTask.CompletedTask;
    }
}

//public sealed class NullTag<T> : ITag<T>
//    where T : notnull
//{
//    private NullTag() { }
//    public static NullTag<T> Instance { get; } = new();


//    public ICodec<T> Codec { get; } = NullCodec<T>.Instance;
//    public IDataAddress Address { get; } = NullDataAddress.Instance;
//}
//public sealed class NullCodec<T> : ICodec<T>
//    where T : notnull
//{
//    private NullCodec() { }
//    public static NullCodec<T> Instance { get; } = new();


//    public T Decode(ReadOnlySpan<byte> raw) => default!;
//    public int Encode(T value, Span<byte> buffer) => 0;
//}
//public sealed class NullDataAddress : IDataAddress
//{
//    private NullDataAddress() { }
//    public static NullDataAddress Instance { get; } = new();

//    public bool Equals(IDataAddress? other) => other is NullDataAddress;
//}