using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using ThabeSoft.Configuration.Mapping;

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


        public IServiceCollection AddTransportConfigMapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMapper>()
            where TMapper : class, IConfigMapper<TransportDescriptor>
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<TransportDescriptor>, TMapper>());
            return descriptors;
        }
    }
}