using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.Transports.Ethernet.Tcp;
using ThabeSoft.Transports.Ethernet.Udp;
using ThabeSoft.Transports.DependencyInjection;

namespace ThabeSoft.Transports.Ethernet.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 提供以太传输能力
        /// </summary>
        public IServiceCollection AddEthernetTransport()
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ITransportConnectionResolver, TcpTransportConnectionResolver>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ITransportConnectionResolver, UdpTransportConnectionResolver>());

            return descriptors;
        }

        /// <summary>
        /// 提供以太配置映射能力
        /// </summary>
        public IServiceCollection AddEthernetTransportConfig()
        {
            descriptors.AddTransportConfigMapper<IpTransportDescriptorConfigMapper>();
            return descriptors;
        }
    }
}
