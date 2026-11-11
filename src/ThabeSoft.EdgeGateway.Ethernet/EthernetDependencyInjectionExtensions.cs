using ThabeSoft.EdgeGateway.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.EdgeGateway;

public static class EthernetDependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加以太网模块
        /// </summary>
        public IServiceCollection AddEthernet()
        {
            descriptors.AddSingleton<ITransportConnectionFactory, TcpTransportConnectionFactory>();
            descriptors.AddSingleton<ITransportConnectionFactory, UdpTransportConnectionFactory>();

            return descriptors;
        }
    }
}
