using System.Net.Sockets;

namespace ThabeSoft.EdgeGateway.Channels;

internal sealed class UdpChannel(IpChannelDescriptor descriptor) : IChannel
{
    private readonly UdpClient _udpClient = new();


    public ChannelAddress Address { get; } = descriptor.Address;
    public bool IsOpen { get; private set; }


    public ValueTask OpenAsync(CancellationToken cancellationToken = default)
    {
        if (IsOpen) return ValueTask.CompletedTask;
        IsOpen = true;

        _udpClient.Connect(descriptor.IPAddress, descriptor.Port);
        return ValueTask.CompletedTask;
    }
    public ValueTask CloseAsync(CancellationToken cancellationToken = default)
    {
        if (!IsOpen) return ValueTask.CompletedTask;
        IsOpen = false;

        _udpClient.Close();
        return ValueTask.CompletedTask;
    }


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


    public ValueTask DisposeAsync()
    {
        _udpClient.Dispose();
        return ValueTask.CompletedTask;
    }
}