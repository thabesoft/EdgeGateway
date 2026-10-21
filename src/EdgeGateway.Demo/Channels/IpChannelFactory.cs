using System.Net;
using System.Net.Sockets;

namespace EdgeGateway.Channels;


public sealed record IpChannelDescriptor : IChannelDescriptor
{
    public required IpConnectionType Connection { get; init; }
    public required IPAddress IPAddress { get; init; }
    public required int Port { get; init; }


    private IpChannelDescriptor() { }

    public static IpChannelDescriptor Tcp(IPAddress address, int port)
    {
        return new()
        {
            IPAddress = address,
            Port = port,
            Connection = IpConnectionType.Tcp,
        };
    }

    public static IpChannelDescriptor Udp(IPAddress address, int port)
    {
        return new()
        {
            IPAddress = address,
            Port = port,
            Connection = IpConnectionType.Udp
        };
    }
}


public enum IpConnectionType
{
    Tcp,

    Udp
}



public sealed class TcpChannelFactory : IChannelFactory
{
    public bool CanCreate(IChannelDescriptor descriptor)
    {
        return descriptor is IpChannelDescriptor ip && ip.Connection == IpConnectionType.Tcp;
    }

    public IChannel Create(IChannelDescriptor descriptor)
    {
        if (descriptor is not IpChannelDescriptor ip || ip.Connection != IpConnectionType.Tcp)
            throw new InvalidOperationException();

        return new TcpChannel(ip);
    }
}


internal sealed class TcpChannel(IpChannelDescriptor descriptor) : IChannel
{
    private readonly TcpClient _tcpClient = new();

    public ChannelAddress Address { get; } = ChannelAddress.Create(ConnectionType.Tcp, $"{descriptor.IPAddress}:{descriptor.Port}");


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



public sealed class UdpChannelFactory : IChannelFactory
{
    public bool CanCreate(IChannelDescriptor descriptor)
    {
        return descriptor is IpChannelDescriptor ip && ip.Connection == IpConnectionType.Udp;
    }

    public IChannel Create(IChannelDescriptor descriptor)
    {
        if (descriptor is not IpChannelDescriptor ip || ip.Connection != IpConnectionType.Udp)
            throw new InvalidOperationException();

        return new UdpChannel(ip);
    }
}

internal sealed class UdpChannel(IpChannelDescriptor descriptor) : IChannel
{
    private readonly UdpClient _udpClient = new();

    public ChannelAddress Address { get; } = ChannelAddress.Create(ConnectionType.Tcp, $"{descriptor.IPAddress}:{descriptor.Port}");


    public ValueTask OpenAsync(CancellationToken cancellationToken = default)
    {
        _udpClient.Connect(descriptor.IPAddress, descriptor.Port);
        return ValueTask.CompletedTask;
    }
    public ValueTask CloseAsync(CancellationToken cancellationToken = default)
    {
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