using EdgeGateway.Channel;
using System.Net;

namespace EdgeGateway.Ethernet;


public sealed record IpChannelDescriptor : IChannelDescriptor
{
    public required IpConnectionType Connection { get; init; }
    public required IPAddress IPAddress { get; init; }
    public required int Port { get; init; }


    internal IpChannelDescriptor() { }
    public static IpChannelDescriptorBuilder Builder() => new();
}


public sealed class IpChannelDescriptorBuilder : IChannelDescriptor
{
    private IpConnectionType _connectionType;
    private IPAddress? _iPAddress;
    private int _port;


    /// <summary>
    /// 使用 Tcp
    /// </summary>
    public IpChannelDescriptorBuilder Tcp()
    {
        _connectionType = IpConnectionType.Tcp;
        return this;
    }

    /// <summary>
    /// 使用 Udp
    /// </summary>
    public IpChannelDescriptorBuilder Udp()
    {
        _connectionType = IpConnectionType.Udp;
        return this;
    }

    /// <summary>
    /// 设置 Ip 地址
    /// </summary>
    /// <param name="iPAddress">ip地址</param>
    /// <param name="port">ip端口, 范围 [0~63365] </param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IpChannelDescriptorBuilder Address(IPAddress iPAddress, int port)
    {
        ArgumentNullException.ThrowIfNull(iPAddress);
        if (port is < 0 or > 65535)
            throw new ArgumentOutOfRangeException(nameof(port));

        _iPAddress = iPAddress;
        _port = port;

        return this;
    }


    public IpChannelDescriptor Build()
    {
        ArgumentNullException.ThrowIfNull(_iPAddress);


        return new()
        {
            Connection = _connectionType,
            IPAddress = _iPAddress,
            Port = _port
        };
    }
}