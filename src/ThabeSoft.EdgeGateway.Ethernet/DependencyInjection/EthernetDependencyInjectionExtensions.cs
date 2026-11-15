using ThabeSoft.EdgeGateway.Transports;
using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.EdgeGateway.Transports.Tcp;
using ThabeSoft.EdgeGateway.Transports.Udp;

namespace ThabeSoft.EdgeGateway.DependencyInjection;


public static class EthernetDependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加以太网模块
        /// </summary>
        public IServiceCollection AddEdgeGatewayEthernet()
        {
            descriptors.AddSingleton<ITransportConnectionFactory, TcpTransportConnectionFactory>();
            descriptors.AddSingleton<ITransportConnectionFactory, UdpTransportConnectionFactory>();

            return descriptors;
        }
    }
}
