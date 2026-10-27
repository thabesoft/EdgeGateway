using EdgeGateway.Config;
using EdgeGateway.Ethernet;
using System.Net;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class ConfigBuilderExtensions
{
    extension(PortConfigBuilder portConfigBuilder)
    {
        /// <summary>
        /// 使用 Tcp 通道
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public PortConfigBuilder Tcp(IPAddress address, int port)
        {
            var builder = IpChannelDescriptor.Builder()
                .Address(address, port)
                .Tcp();

            return portConfigBuilder.SetDescriptor(builder.Build());
        }
        /// <summary>
        /// 使用 Tcp 通道
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public PortConfigBuilder Tcp(string address, int port) =>
            portConfigBuilder.Tcp(IPAddress.Parse(address), port);


        /// <summary>
        /// 使用 Udp 通道
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public PortConfigBuilder Udp(IPAddress address, int port)
        {
            var builder = IpChannelDescriptor.Builder()
               .Address(address, port)
               .Udp();

            return portConfigBuilder.SetDescriptor(builder.Build());
        }
        /// <summary>
        /// 使用 Udp 通道
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public PortConfigBuilder Udp(string address, int port) =>
            portConfigBuilder.Udp(IPAddress.Parse(address), port);
    }
}