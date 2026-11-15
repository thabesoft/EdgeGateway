using System.Net;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;


public static class Ethernet
{
    public static IpTransportDescriptor Tcp(IPEndPoint iPEndPoint, RetryDescriptor? retry = null)
        => new(IpTransportType.Tcp, iPEndPoint) { Retry = retry ?? RetryDescriptor.Default };
    public static IpTransportDescriptor Tcp(IPAddress ip, int port, RetryDescriptor? retry = null)
        => Tcp(new IPEndPoint(ip, port), retry);
    public static IpTransportDescriptor Tcp(string ip, int port, RetryDescriptor? retry = null)
        => Tcp(IPAddress.Parse(ip), port);


    public static IpTransportDescriptor Udp(IPEndPoint iPEndPoint, RetryDescriptor? retry = null)
        => new(IpTransportType.Udp, iPEndPoint) { Retry = retry ?? RetryDescriptor.Default };
    public static IpTransportDescriptor Udp(IPAddress ip, int port, RetryDescriptor? retry = null)
        => Udp(new IPEndPoint(ip, port), retry);
    public static IpTransportDescriptor Udp(string ip, int port, RetryDescriptor? retry = null)
        => Udp(IPAddress.Parse(ip), port, retry);
}