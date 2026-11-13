using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace ThabeSoft.EdgeGateway.Transport;


public sealed record IpTransportDescriptor : ITransportDescriptor
{
    public ConnectionEndpoint Endpoint { get; }
    public IpTransportType TransportType { get; }
    public IPAddress IPAddress { get; }
    public int Port { get; }
    // 由于 Address 和 Port 属性可更改, 所以每次返回新的对象
    public IPEndPoint IPEndPoint => new(IPAddress, Port);



    public IpTransportDescriptor(IpTransportType transportType, IPEndPoint iPEndPoint)
    {
        if (!ValidateTransportType(transportType, out var errorMessage))
            throw new ArgumentOutOfRangeException(nameof(transportType), errorMessage);

        ArgumentNullException.ThrowIfNull(iPEndPoint);
        ArgumentNullException.ThrowIfNull(iPEndPoint.Address);

        Endpoint = ConnectionEndpoint.Ethernet(iPEndPoint);
        TransportType = transportType;
        IPAddress = iPEndPoint.Address;
        Port = iPEndPoint.Port;
    }
    public bool Equals(ITransportDescriptor other)
    {
        if (other is not IpTransportDescriptor ip) return false;
        return ip.Equals(this);
    }
    // Tcp(192.168.1.1:1234)
    public override string ToString() => $"{TransportType}({IPEndPoint})";



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
    public static bool ValidateTransportType(IpTransportType type, [NotNullWhen(false)] out string? errorMessage)
    {
        if (!Enum.IsDefined(type))
        {
            errorMessage = $"不支持的传输方式, 仅支持 [{IpTransportType.Tcp}, {IpTransportType.Udp}]";
            return false;
        }
        errorMessage = null;
        return true;
    }
}