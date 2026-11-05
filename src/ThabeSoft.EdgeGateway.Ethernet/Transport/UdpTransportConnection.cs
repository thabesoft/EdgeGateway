using System.Net.Sockets;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.Transport;

internal sealed class UdpTransportConnection(IpTransportDescriptor descriptor) : LifecycleStateMachine, ITransportConnection
{
    private readonly UdpClient _udpClient = new();


    public ConnectionEndpoint Endpoint { get; } = descriptor.Address;


    public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var result = await _udpClient.ReceiveAsync(cancellationToken);
        var datagram = result.Buffer;

        if (datagram.Length > buffer.Length)
        {
            throw new InvalidOperationException($"数据报 {datagram.Length} 字节超过缓冲区 {buffer.Length} 字节。");
        }

        datagram.CopyTo(buffer.Span);
        return datagram.Length;
    }
    public async ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        await _udpClient.SendAsync(data, cancellationToken);
    }



    protected override ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        _udpClient.Connect(descriptor.IPAddress, descriptor.Port);
        return ValueTask.CompletedTask;
    }
    protected override ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        _udpClient.Close();
        return ValueTask.CompletedTask;
    }
    protected override ValueTask OnDisposeAsync()
    {
        _udpClient.Dispose();
        return ValueTask.CompletedTask;
    }
}