using System.Net;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Options;


public static class OptionsExtensions
{
    extension(IGatewayOptionsBuilder builder)
    {
        public IGatewayOptionsBuilder AddTcpPort(IPEndPoint iPEndPoint, Action<IPortOptionsBuilder>? portBuildAction = null) =>
            builder.AddPort(x => { portBuildAction?.Invoke(x); x.UseTcp(iPEndPoint); });
        public IGatewayOptionsBuilder AddTcpPort(IPAddress iPAddress, int port, Action<IPortOptionsBuilder>? portBuildAction = null) =>
            builder.AddPort(x => { portBuildAction?.Invoke(x); x.UseTcp(iPAddress, port); });
        public IGatewayOptionsBuilder AddTcpPort(string iPAddress, int port, Action<IPortOptionsBuilder>? portBuildAction = null) =>
            builder.AddPort(x => { portBuildAction?.Invoke(x); x.UseTcp(iPAddress, port); });


        public IGatewayOptionsBuilder AddUdpPort(IPEndPoint iPEndPoint, Action<IPortOptionsBuilder>? portBuildAction = null) =>
            builder.AddPort(x => { portBuildAction?.Invoke(x); x.UseUdp(iPEndPoint); });
        public IGatewayOptionsBuilder AddUdpPort(IPAddress iPAddress, int port, Action<IPortOptionsBuilder>? portBuildAction = null) =>
            builder.AddPort(x => { portBuildAction?.Invoke(x); x.UseUdp(iPAddress, port); });
        public IGatewayOptionsBuilder AddUdpPort(string iPAddress, int port, Action<IPortOptionsBuilder>? portBuildAction = null) =>
            builder.AddPort(x => { portBuildAction?.Invoke(x); x.UseUdp(iPAddress, port); });
    }


    extension(IPortOptionsBuilder builder)
    {
        public IPortOptionsBuilder UseTcp(IPEndPoint iPEndPoint) =>
            builder.WithTransport(IpTransportDescriptor.Tcp(iPEndPoint));
        public IPortOptionsBuilder UseTcp(IPAddress iPAddress, int port) =>
            builder.UseTcp(new IPEndPoint(iPAddress, port));
        public IPortOptionsBuilder UseTcp(string iPAddress, int port) =>
            builder.UseTcp(IPAddress.Parse(iPAddress), port);


        public IPortOptionsBuilder UseUdp(IPEndPoint iPEndPoint) =>
            builder.WithTransport(IpTransportDescriptor.Udp(iPEndPoint));
        public IPortOptionsBuilder UseUdp(IPAddress iPAddress, int port) =>
            builder.UseUdp(new IPEndPoint(iPAddress, port));
        public IPortOptionsBuilder UseUdp(string iPAddress, int port) =>
            builder.UseUdp(IPAddress.Parse(iPAddress), port);
    }
}
