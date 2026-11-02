using System.Net;

namespace ThabeSoft.EdgeGateway.Channels;


public sealed record IpChannelDescriptor : IChannelDescriptor
{
    public required ChannelId Id { get; init; }
    public required IpConnectionType ConnectionType { get; init; }
    public required IPAddress IPAddress { get; init; }
    public required int Port { get; init; }

    public IPEndPoint IPEndPoint => new(IPAddress, Port);


    ConnectionType IChannelDescriptor.ConnectionType => Channels.ConnectionType.Ethernet;

    internal IpChannelDescriptor() { }
    public static IpChannelDescriptor Create(ChannelId id, IpConnectionType connectionType, IPEndPoint iPEndPoint)
    {
        ArgumentNullException.ThrowIfNull(id);

        if (connectionType is not (IpConnectionType.Tcp or IpConnectionType.Udp))
            throw new ArgumentOutOfRangeException(nameof(connectionType), $"不支持的Ip连接方式: {connectionType}");

        ArgumentNullException.ThrowIfNull(iPEndPoint);
        ArgumentNullException.ThrowIfNull(iPEndPoint.Address);


        return new()
        {
            Id = id,
            ConnectionType = connectionType,
            IPAddress = iPEndPoint.Address,
            Port = iPEndPoint.Port
        };
    }
    public static IpChannelDescriptor Create(IpConnectionType connectionType, IPEndPoint iPEndPoint) =>
        Create(ChannelId.Create(), connectionType, iPEndPoint);


    public static IpChannelDescriptor Tcp(ChannelId id, IPEndPoint iPEndPoint) =>
        Create(id, IpConnectionType.Tcp, iPEndPoint);
    public static IpChannelDescriptor Tcp(IPEndPoint iPEndPoint) =>
        Tcp(ChannelId.Create(),iPEndPoint);

    public static IpChannelDescriptor Udp(ChannelId id, IPEndPoint iPEndPoint) =>
        Create(id, IpConnectionType.Udp, iPEndPoint);
    public static IpChannelDescriptor Udp(IPEndPoint iPEndPoint) =>
        Udp(ChannelId.Create(), iPEndPoint);
}