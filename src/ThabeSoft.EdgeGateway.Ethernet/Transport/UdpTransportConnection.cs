using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using System.Threading.Channels;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.Transport;

internal sealed class UdpTransportConnection(
        IpTransportDescriptor descriptor,
        ILogger<UdpTransportConnection> logger
    ) : ITransportConnection
{
    private readonly IStrictLifecycleStateMachine _stateMachine = Lifecycle.StrictStateMachine(logger);
    private readonly UdpClient _udpClient = new();

    private Task? _receiveLoop;
    private CancellationTokenSource? _receiveCts;
    private Channel<byte[]>? _datagrams;



    public ConnectionEndpoint Endpoint { get; } = descriptor.Endpoint;
    public LifecycleState State => _stateMachine.State;



    public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _stateMachine.ExecuteAsync
        (
            method: async ct =>
            {
                if (_datagrams is null)
                    return 0;   // 未启动

                if (!await _datagrams.Reader.WaitToReadAsync(ct))
                    return 0;   // Channel 已完成（Stop 了）

                if (!_datagrams.Reader.TryRead(out var datagram))
                    return 0;

                var length = Math.Min(datagram.Length, buffer.Length);
                datagram.AsSpan(0, length).CopyTo(buffer.Span);
                return length;
            },
            cancellationToken: cancellationToken
        );
    }
    public async ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        await _stateMachine.ExecuteAsync
        (
            method: ct => _udpClient.SendAsync(data, ct),
            cancellationToken: cancellationToken
        );
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);



    private ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        _udpClient.Connect(descriptor.IPAddress, descriptor.Port);

        // 每次启动创建新的 Channel 和 CTS
        _datagrams = Channel.CreateUnbounded<byte[]>();
        _receiveCts = new CancellationTokenSource();

        // 启动后台接收循环
        _receiveLoop = Task.Run(() => ReceiveLoopAsync(_receiveCts.Token), _receiveCts.Token);


        return ValueTask.CompletedTask;
    }
    private async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        _udpClient.Close();

        // 取消接收循环
        _receiveCts?.Cancel();

        // 等接收循环退出
        if (_receiveLoop is not null)
        {
            try { await _receiveLoop; }
            catch (OperationCanceledException) { }
            _receiveLoop = null;
        }

        // 完成 Channel 写入，让 ReadAsync 的 WaitToReadAsync 返回 false
        _datagrams?.Writer.TryComplete();

        // 清空队列里剩余的数据报
        if (_datagrams is not null)
        {
            while (_datagrams.Reader.TryRead(out _)) { }
        }

        _receiveCts?.Dispose();
        _receiveCts = null;
        _datagrams = null;
    }
    private async ValueTask OnDisposeAsync()
    {
        try
        {
            await OnStopAsync(CancellationToken.None);
        }
        catch
        {

        }

        // 2. 释放底层 UdpClient
        try { _udpClient.Dispose(); }
        catch (Exception ex) { logger.LogError(ex, "释放 UdpClient 失败"); }
    }



    private async Task ReceiveLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var result = await _udpClient.ReceiveAsync(ct);
                _datagrams?.Writer.TryWrite(result.Buffer);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex) { logger.LogError(ex, "Udp 客户端接收异常"); }
        }
    }
}