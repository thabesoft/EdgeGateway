using FluentResults;

namespace EdgeGateway.Ports;


/// <summary>
/// 表示一个可被打开和关闭的物理或逻辑通信端口，例如串口、TCP 客户端或 USB 设备 <br/>
/// 在提供原始字节流收发能力的基础上增加了端口生命周期管理（打开、关闭）和状态查询（是否已打开）功能
/// </summary>
public interface IPort : IAsyncDisposable
{
    PortAddress Address { get; }
    bool IsOpen { get; }


    ValueTask<Result> OpenAsync(CancellationToken cancellationToken = default);
    ValueTask<Result> CloseAsync(CancellationToken cancellationToken = default);


    /// <summary>
    /// 读取指定缓冲区大小的字节
    /// </summary>
    ValueTask<Result> ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
    /// <summary>
    /// 写入全部字节
    /// </summary>
    ValueTask<Result> WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);
}


public abstract class Port(PortAddress portAddress) : IPort
{
    public PortAddress Address => portAddress;
    public bool IsOpen { get; private set; }


    async ValueTask<Result> IPort.OpenAsync(CancellationToken cancellationToken)
    {
        if (IsOpen) return Result.Fail("当前已开启");

        var result = await OpenAsync(cancellationToken);
        if (result.IsFailed) return result;

        IsOpen = true;
        return Result.Ok();
    }
    async ValueTask<Result> IPort.CloseAsync(CancellationToken cancellationToken)
    {
        if (!IsOpen) return Result.Ok().WithSuccess("端口已关闭");

        var result = await CloseAsync(cancellationToken);
        if (result.IsFailed) return result;

        IsOpen = false;
        return Result.Ok();
    }


    protected abstract ValueTask<Result> OpenAsync(CancellationToken cancellationToken = default);
    protected abstract ValueTask<Result> CloseAsync(CancellationToken cancellationToken = default);
    public abstract ValueTask DisposeAsync();


    public abstract ValueTask<Result> ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
    public abstract ValueTask<Result> WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);
}

public abstract class StreamPort(PortAddress portAddress) : Port(portAddress)
{
    protected abstract Stream Stream { get; }

    public override ValueTask<Result> ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return Result.Try(() => Stream.ReadExactlyAsync(buffer, cancellationToken));
    }
    public override ValueTask<Result> WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        return Result.Try(() => Stream.WriteAsync(data, cancellationToken));
    }
}