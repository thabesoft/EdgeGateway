using System.Net;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.Configuration;


public static class GatewayExtensions
{
    extension(IGateway gateway)
    {
        public ValueTask<IPort> AddTcpPortAsync(IPEndPoint iPEndPoint, CancellationToken cancellationToken = default) =>
            gateway.AddPortAsync(Ethernet.Tcp(iPEndPoint), cancellationToken);
        public ValueTask<IPort> AddTcpPort(IPAddress ip, int port, CancellationToken cancellationToken = default) =>
            gateway.AddPortAsync(Ethernet.Tcp(ip, port), cancellationToken);
        public ValueTask<IPort> AddTcpPort(string ip, int port, CancellationToken cancellationToken = default) =>
            gateway.AddPortAsync(Ethernet.Tcp(ip, port), cancellationToken);


        public ValueTask<IPort> AddUdpPortAsync(IPEndPoint iPEndPoint, CancellationToken cancellationToken = default) =>
            gateway.AddPortAsync(Ethernet.Udp(iPEndPoint), cancellationToken);
        public ValueTask<IPort> AddUdpPortAsync(IPAddress ip, int port, CancellationToken cancellationToken = default) =>
            gateway.AddPortAsync(Ethernet.Udp(ip, port), cancellationToken);
        public ValueTask<IPort> AddUdpPortAsync(string ip, int port, CancellationToken cancellationToken = default) =>
            gateway.AddPortAsync(Ethernet.Udp(ip, port), cancellationToken);
    }
}
