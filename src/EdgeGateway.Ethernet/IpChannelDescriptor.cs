using EdgeGateway.Channel;
using System.Net;

namespace EdgeGateway.Ethernet;


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