using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace ThabeSoft.EdgeGateway.Channels;


public sealed record IpChannelDescriptor : IChannelDescriptor
{
    public required ChannelAddress Address { get; init; }
    public required IpConnectionType ConnectionType { get; init; }
    public required IPAddress IPAddress { get; init; }
    public required int Port { get; init; }


    public IPEndPoint IPEndPoint => new(IPAddress, Port);



    internal IpChannelDescriptor() { }
    public static IpChannelDescriptor Create(IpConnectionType connectionType, IPEndPoint iPEndPoint)
    {
        if (!ValidateConnectionType(connectionType, out var errorMessage))
            throw new ArgumentOutOfRangeException(nameof(connectionType), errorMessage);

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




    public static bool ValidateIp(string ipString, [NotNullWhen(false)] out string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(ipString))
        {
            errorMessage = "请输入Ip";
            return false;
        }

        if (!IPAddress.TryParse(ipString, out var address))
        {
            errorMessage = "格式不正确，例如：192.168.1.10";
            return false;
        }

        errorMessage = null;
        return true;
    }
    public static bool ValidatePort(int port, [NotNullWhen(false)] out string? errorMessage)
    {
        if (port is < IPEndPoint.MinPort or > IPEndPoint.MaxPort)
        {
            errorMessage = $"端口号超出范围, 应该在 [{IPEndPoint.MinPort}~{IPEndPoint.MaxPort}] 之中";
            return false;
        }

        errorMessage = null;
        return true;
    }
    public static bool ValidateConnectionType(IpConnectionType type, [NotNullWhen(false)] out string? errorMessage)
    {
        if (!Enum.IsDefined(type))
        {
            errorMessage = $"不支持的连接方式, 仅支持 [{IpConnectionType.Tcp}, {IpConnectionType.Udp}]";
            return false;
        }
        errorMessage = null;
        return true;
    }
}