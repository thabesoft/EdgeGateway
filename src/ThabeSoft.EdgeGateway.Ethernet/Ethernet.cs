using System.Net;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;


public static class Ethernet
{
    public static IpTransportDescriptor Tcp(IPEndPoint iPEndPoint)
        => IpTransportDescriptor.Create(IpTransportType.Tcp, iPEndPoint);
    public static IpTransportDescriptor Tcp(IPAddress ip, int port)
        => Tcp(new IPEndPoint(ip, port));
    public static IpTransportDescriptor Tcp(string ip, int port)
        => Tcp(IPAddress.Parse(ip), port);


    public static IpTransportDescriptor Udp(IPEndPoint iPEndPoint)
        => IpTransportDescriptor.Create(IpTransportType.Udp, iPEndPoint);
    public static IpTransportDescriptor Udp(IPAddress ip, int port)
        => Udp(new IPEndPoint(ip, port));
    public static IpTransportDescriptor Udp(string ip, int port)
        => Udp(IPAddress.Parse(ip), port);
}