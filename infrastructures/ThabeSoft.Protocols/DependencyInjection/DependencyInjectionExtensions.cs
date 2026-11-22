using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using ThabeSoft.Configuration.Mapping;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.Protocols.Accessor;

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


        /// <summary>
        /// 添加指定类型的协议配置映射器
        /// </summary>
        public IServiceCollection AddProtocolConfigMapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMapper>()
            where TMapper : class, IConfigMapper<ProtocolDescriptor>
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<ProtocolDescriptor>, TMapper>());
            return descriptors;
        }

        /// <summary>
        /// 添加指定类型的协议地址配置映射器
        /// </summary>
        public IServiceCollection AddProtocolAddressMapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMapper>()
           where TMapper : class, IConfigMapper<ProtocolAddress>
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<ProtocolAddress>, TMapper>());
            return descriptors;
        }
    }
}