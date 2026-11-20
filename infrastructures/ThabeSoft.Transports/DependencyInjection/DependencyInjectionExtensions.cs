using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ThabeSoft.Transports.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 提供网关传输管理能力
        /// </summary>
        public IServiceCollection AddTransports()
        {
            descriptors.TryAddSingleton<ITransportConnectionFactory, TransportConnectionFactory>();
            return descriptors;
        }
    }
}