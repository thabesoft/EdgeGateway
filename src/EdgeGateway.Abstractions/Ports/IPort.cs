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

public interface IBufferDiscardable
{
    void DiscardInBuffer();
}