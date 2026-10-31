using ThabeSoft.EdgeGateway.Channel;
using System.Net;

namespace ThabeSoft.EdgeGateway.Ethernet;


public sealed record IpChannelDescriptor : IChannelDescriptor
{
    public required ChannelAddress Address { get; init; }
    public required IpConnectionType Connection { get; init; }
    public required IPAddress IPAddress { get; init; }
    public required int Port { get; init; }


    internal IpChannelDescriptor() { }
    public static IpChannelDescriptorBuilder Builder() => new();
}


public sealed class IpChannelDescriptorBuilder
{
    private IpConnectionType _connectionType;
    private IPAddress? _iPAddress;
    private int _port;


    /// <summary>
    /// 设置连接方式
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IpChannelDescriptorBuilder Connection(IpConnectionType type)
    {
        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(nameof(type), "不支持的连接类型");

        _connectionType = type;
        return this;
    }

    /// <summary>
    /// 使用 Tcp
    /// </summary>
    public IpChannelDescriptorBuilder Tcp() => Connection(IpConnectionType.Tcp);

    /// <summary>
    /// 使用 Udp
    /// </summary>
    public IpChannelDescriptorBuilder Udp() => Connection(IpConnectionType.Udp);

    /// <summary>
    /// 设置端口
    /// </summary>
    /// <param name="port">ip端口, 范围 [0~63365] </param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IpChannelDescriptorBuilder Port(int port)
    {
        if (port is < 0 or > 65535)
            throw new ArgumentOutOfRangeException(nameof(port));

        _port = port;
        return this;
    }


    /// <summary>
    /// 设置 Ip 地址
    /// </summary>
    /// <param name="iPAddress">ip地址</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IpChannelDescriptorBuilder Address(IPAddress iPAddress)
    {
        ArgumentNullException.ThrowIfNull(iPAddress);
        _iPAddress = iPAddress;
        return this;
    }

    public IpChannelDescriptorBuilder Address(IPEndPoint iPEndPoint)
    {
        ArgumentNullException.ThrowIfNull(iPEndPoint);

        _iPAddress = iPEndPoint.Address;
        _port = iPEndPoint.Port;

        return this;
    }



    public IpChannelDescriptor Build()
    {
        ArgumentNullException.ThrowIfNull(_iPAddress);

        return new()
        {
            Connection = _connectionType,
            IPAddress = _iPAddress,
            Port = _port,
            Address = ChannelAddress.Create(ConnectionType.Ethernet, $"{_iPAddress}:{_port}")
        };
    }
}