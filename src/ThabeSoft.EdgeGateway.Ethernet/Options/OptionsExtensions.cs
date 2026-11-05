using System.Net;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Configuration;


public static class OptionsExtensions
{
    extension(IGatewayOptionsConfigurator builder)
    {
        public IGatewayOptionsConfigurator AddTcpPort(IPEndPoint iPEndPoint, Action<IPortOptionsConfigurator>? portBuildAction = null) =>
            builder.AddPort(x => { portBuildAction?.Invoke(x); x.UseTcp(iPEndPoint); });
        public IGatewayOptionsConfigurator AddTcpPort(IPAddress iPAddress, int port, Action<IPortOptionsConfigurator>? portBuildAction = null) =>
            builder.AddPort(x => { portBuildAction?.Invoke(x); x.UseTcp(iPAddress, port); });
        public IGatewayOptionsConfigurator AddTcpPort(string iPAddress, int port, Action<IPortOptionsConfigurator>? portBuildAction = null) =>
            builder.AddPort(x => { portBuildAction?.Invoke(x); x.UseTcp(iPAddress, port); });


        public IGatewayOptionsConfigurator AddUdpPort(IPEndPoint iPEndPoint, Action<IPortOptionsConfigurator>? portBuildAction = null) =>
            builder.AddPort(x => { portBuildAction?.Invoke(x); x.UseUdp(iPEndPoint); });
        public IGatewayOptionsConfigurator AddUdpPort(IPAddress iPAddress, int port, Action<IPortOptionsConfigurator>? portBuildAction = null) =>
            builder.AddPort(x => { portBuildAction?.Invoke(x); x.UseUdp(iPAddress, port); });
        public IGatewayOptionsConfigurator AddUdpPort(string iPAddress, int port, Action<IPortOptionsConfigurator>? portBuildAction = null) =>
            builder.AddPort(x => { portBuildAction?.Invoke(x); x.UseUdp(iPAddress, port); });
    }


    extension(IPortOptionsConfigurator builder)
    {
        public IPortOptionsConfigurator UseTcp(IPEndPoint iPEndPoint) =>
            builder.WithTransport(IpTransportDescriptor.Tcp(iPEndPoint));
        public IPortOptionsConfigurator UseTcp(IPAddress iPAddress, int port) =>
            builder.UseTcp(new IPEndPoint(iPAddress, port));
        public IPortOptionsConfigurator UseTcp(string iPAddress, int port) =>
            builder.UseTcp(IPAddress.Parse(iPAddress), port);


        public IPortOptionsConfigurator UseUdp(IPEndPoint iPEndPoint) =>
            builder.WithTransport(IpTransportDescriptor.Udp(iPEndPoint));
        public IPortOptionsConfigurator UseUdp(IPAddress iPAddress, int port) =>
            builder.UseUdp(new IPEndPoint(iPAddress, port));
        public IPortOptionsConfigurator UseUdp(string iPAddress, int port) =>
            builder.UseUdp(IPAddress.Parse(iPAddress), port);
    }
}
