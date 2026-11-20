using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.Protocols.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 提供网关协议管理能力
        /// </summary>
        public IServiceCollection AddProtocols()
        {
            descriptors.TryAddSingleton<IProtocolAccessorFactory, ProtocolAccessorFactory>();
            return descriptors;
        }
    }
}