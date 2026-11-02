using System.Net;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.Channels;


public sealed record IpChannelDescriptor : IChannelDescriptor
{
    public required ChannelAddress Address { get; init; }
    public required IpConnectionType ConnectionType { get; init; }
    public required IPAddress IPAddress { get; init; }
    public required int Port { get; init; }


    public IPEndPoint IPEndPoint => new(IPAddress, Port);



    internal IpChannelDescriptor() { }
    public static IpChannelDescriptorBuilder Builder() => new();


    public static IpChannelDescriptor Create(IpConnectionType connectionType, IPEndPoint iPEndPoint)
    {
        if (connectionType is not (IpConnectionType.Tcp or IpConnectionType.Udp))
            throw new ArgumentOutOfRangeException(nameof(connectionType), $"不支持的Ip连接方式: {connectionType}");

        ArgumentNullException.ThrowIfNull(iPEndPoint);
        ArgumentNullException.ThrowIfNull(iPEndPoint.Address);

        return new()
        {
            Address = ChannelAddress.Ethernet(iPEndPoint),
            ConnectionType = connectionType,
            IPAddress = iPEndPoint.Address,
            Port = iPEndPoint.Port
        };
    }
    public static IpChannelDescriptor Tcp(IPEndPoint iPEndPoint) =>
        Create(IpConnectionType.Tcp, iPEndPoint);
    public static IpChannelDescriptor Udp(IPEndPoint iPEndPoint) =>
        Create(IpConnectionType.Udp, iPEndPoint);
}


public sealed record IpChannelDescriptorBuilder
{
    private IPAddress? _address;
    private int? _port;
    private IpConnectionType? _connectionType;


    public IpChannelDescriptorBuilder Address(IPAddress address)
    {
        if (address is null)
            throw new ArgumentException("IP 地址不可为空", nameof(address));

        _address = address;
        return this;
    }

    public IpChannelDescriptorBuilder Address(string addressString)
    {
        if (string.IsNullOrWhiteSpace(addressString))
            throw new ArgumentException("IP 地址不可为空", nameof(addressString));

        if (!IPAddress.TryParse(addressString, out var address))
            throw new FormatException("Ip 地址格式错误");

        return Address(address);
    }

    public IpChannelDescriptorBuilder Port(int port)
    {
        if (port is < IPEndPoint.MinPort or > IPEndPoint.MaxPort)
            throw new ArgumentOutOfRangeException(nameof(port), $"端口号超出范围 [{IPEndPoint.MinPort}~{IPEndPoint.MaxPort}]");

        _port = port;
        return this;
    }


    public IpChannelDescriptorBuilder ConnectionType(IpConnectionType type)
    {
        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(nameof(type), "不支持的连接方式");

        _connectionType = type;
        return this;
    }

    public IpChannelDescriptor Build()
    {
        if (_address is null)
            throw new InvalidOperationException("未设置 [Ip地址]");

        if (_port is null)
            throw new InvalidOperationException("未设置 [端口]");

        if (_connectionType is null)
            throw new InvalidOperationException("未设置 [连接类型]");

        return new()
        {
            ConnectionType = _connectionType.Value,
            Port = _port.Value,
            IPAddress = _address,
            Address = ChannelAddress.Ethernet(new(_address, _port.Value))
        };
    }
}