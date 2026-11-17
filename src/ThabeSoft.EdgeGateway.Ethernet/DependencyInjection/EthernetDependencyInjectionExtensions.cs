using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Transports;
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
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ITransportConnectionResolver, TcpTransportConnectionResolver>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ITransportConnectionResolver, UdpTransportConnectionResolver>());

            return descriptors;
        }
    }
}
