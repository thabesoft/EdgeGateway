using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Ethernet.Tcp;
using ThabeSoft.EdgeGateway.Ethernet.Udp;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Ethernet.DependencyInjection;


public static class DependencyInjectionExtensions
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


        public IServiceCollection AddEdgeGatewayEthernetConfig()
        {
            descriptors.AddTransportConfigMapper<IpTransportDescriptorConfigMapper>();
            return descriptors;
        }
    }
}
