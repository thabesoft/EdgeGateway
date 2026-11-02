using System.Net.Sockets;
using ThabeSoft.EdgeGateway.IO;

namespace ThabeSoft.EdgeGateway.Channels;


internal sealed class TcpChannel(IpChannelDescriptor descriptor) : IChannel
{
    private readonly TcpClient _tcpClient = new();


    public ChannelId Id { get; } = descriptor.Id;
    public ChannelAddress Address { get; } = ChannelAddress.Ethernet(descriptor.IPEndPoint.ToString());
    public bool IsOpen => _tcpClient.Connected;


    public async ValueTask OpenAsync(CancellationToken cancellationToken = default)
    {
        await _tcpClient.ConnectAsync(descriptor.IPAddress, descriptor.Port, cancellationToken);
    }
    public ValueTask CloseAsync(CancellationToken cancellationToken = default)
    {
        _tcpClient.Close();
        return ValueTask.CompletedTask;
    }


    public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _tcpClient.GetStream().ReadAsync(buffer, cancellationToken);
    }

    public ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        return _tcpClient.GetStream().WriteAsync(data, cancellationToken);
    }


    public ValueTask DisposeAsync()
    {
        _tcpClient.Dispose();
        return ValueTask.CompletedTask;
    }
}
