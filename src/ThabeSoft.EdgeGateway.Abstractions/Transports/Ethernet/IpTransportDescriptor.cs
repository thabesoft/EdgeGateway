using System.Diagnostics.CodeAnalysis;
using System.Net;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Transports.Ethernet;


public sealed record IpTransportDescriptor : TransportDescriptor
{
    public required IpProtocolType ProtocolType { get; init => field = value.Defined("不支持的Ip协议"); }
    public required IPAddress IP { get; init => field = value.Required("Ip地址不可为空"); }
    public required int Port { get; init => field = value.Positive("端口不可为负数"); }


    // 由于 Address 和 Port 属性可更改, 所以每次返回新的对象
    public IPEndPoint IPEndPoint => new(IP, Port);
    public override ConnectionEndpoint Endpoint => ConnectionEndpoint.Ethernet(IPEndPoint);


    // Tcp(192.168.1.1:1234)
    public override string ToString() => $"{ProtocolType}({IPEndPoint})";



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
    public static bool ValidateTransportType(IpProtocolType type, [NotNullWhen(false)] out string? errorMessage)
    {
        if (!Enum.IsDefined(type))
        {
            errorMessage = $"不支持的传输方式, 仅支持 [{IpProtocolType.Tcp}, {IpProtocolType.Udp}]";
            return false;
        }
        errorMessage = null;
        return true;
    }
}




public sealed class RetryBackoff
{
    private readonly RetryDescriptor _descriptor;

    public RetryBackoff(RetryDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        _descriptor = descriptor;
        CurrentInterval = descriptor.StartInterval;
    }

    public TimeSpan CurrentInterval { get; private set; }


    // 重置重试间隔
    public TimeSpan Reset()
    {
        return CurrentInterval = _descriptor.StartInterval;
    }

    // 获取下一次重试间隔
    public TimeSpan GetNextInterval()
    {
        var nextMs = CurrentInterval.TotalMilliseconds * _descriptor.BackoffMultiplier;

        CurrentInterval = nextMs > _descriptor.MaxInterval.TotalMilliseconds
           ? _descriptor.MaxInterval
           : TimeSpan.FromMilliseconds(nextMs);

        return CurrentInterval;
    }
}