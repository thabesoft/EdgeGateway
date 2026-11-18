using System.Net;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway;


public static class Ethernet
{
    public static IpTransportDescriptor Tcp(IPEndPoint iPEndPoint, RetryDescriptor? retry = null)
        => new() { IP = iPEndPoint.Address, Port = iPEndPoint.Port, ProtocolType = IpProtocolType.Tcp, Retry = retry ?? Retry.Default };
    public static IpTransportDescriptor Tcp(IPAddress ip, int port, RetryDescriptor? retry = null)
        => Tcp(new IPEndPoint(ip, port), retry);
    public static IpTransportDescriptor Tcp(string ip, int port, RetryDescriptor? retry = null)
        => Tcp(IPAddress.Parse(ip), port);


    public static IpTransportDescriptor Udp(IPEndPoint iPEndPoint, RetryDescriptor? retry = null)
        => new() { IP = iPEndPoint.Address, Port = iPEndPoint.Port, ProtocolType = IpProtocolType.Udp, Retry = retry ?? Retry.Default };
    public static IpTransportDescriptor Udp(IPAddress ip, int port, RetryDescriptor? retry = null)
        => Udp(new IPEndPoint(ip, port), retry);
    public static IpTransportDescriptor Udp(string ip, int port, RetryDescriptor? retry = null)
        => Udp(IPAddress.Parse(ip), port, retry);
}