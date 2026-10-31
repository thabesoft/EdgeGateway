using ThabeSoft.EdgeGateway.Configs;
using ThabeSoft.EdgeGateway.Ethernet;
using System.Net;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class ConfigBuilderExtensions
{
    extension(PortConfigBuilder portConfigBuilder)
    {
        public PortConfigBuilder Ethernet(IpConnectionType type, IPAddress address, int port)
        {
            var builder = IpChannelDescriptor.Builder()
                .Address(address)
                .Port(port)
                .Connection(type);

            return portConfigBuilder.SetDescriptor(builder.Build());
        }


        /// <summary>
        /// 使用 Tcp 通道
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public PortConfigBuilder Tcp(IPAddress address, int port) =>
            portConfigBuilder.Ethernet(IpConnectionType.Tcp, address, port);

        /// <summary>
        /// 使用 Tcp 通道
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public PortConfigBuilder Tcp(string address, int port) =>
            portConfigBuilder.Tcp(IPAddress.Parse(address), port);

        public PortConfigBuilder Tcp(IPEndPoint iPEndPoint) =>
            portConfigBuilder.Tcp(iPEndPoint.Address, iPEndPoint.Port);



        /// <summary>
        /// 使用 Udp 通道
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public PortConfigBuilder Udp(IPAddress address, int port) =>
            portConfigBuilder.Ethernet(IpConnectionType.Udp, address, port);

        /// <summary>
        /// 使用 Udp 通道
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public PortConfigBuilder Udp(string address, int port) =>
            portConfigBuilder.Udp(IPAddress.Parse(address), port);

        public PortConfigBuilder Udp(IPEndPoint iPEndPoint) =>
            portConfigBuilder.Udp(iPEndPoint.Address, iPEndPoint.Port);
    }
}