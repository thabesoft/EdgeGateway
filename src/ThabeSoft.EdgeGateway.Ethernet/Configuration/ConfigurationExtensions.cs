using System.Net;

namespace ThabeSoft.EdgeGateway.Configuration;


public static class ConfigurationExtensions
{
    extension(IGatewayConfigurator configurator)
    {
        public IPortConfigurator AddTcpPort(IPEndPoint iPEndPoint) =>
            configurator.AddPort(Ethernet.Tcp(iPEndPoint));
        public IPortConfigurator AddTcpPort(IPAddress ip, int port) =>
            configurator.AddPort(Ethernet.Tcp(ip, port));
        public IPortConfigurator AddTcpPort(string ip, int port) =>
            configurator.AddPort(Ethernet.Tcp(ip, port));


        public IPortConfigurator AddUdpPort(IPEndPoint iPEndPoint) =>
            configurator.AddPort(Ethernet.Udp(iPEndPoint));
        public IPortConfigurator AddUdpPort(IPAddress ip, int port) =>
            configurator.AddPort(Ethernet.Udp(ip, port));
        public IPortConfigurator AddUdpPort(string ip, int port) =>
            configurator.AddPort(Ethernet.Udp(ip, port));
    }
}
