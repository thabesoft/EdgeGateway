using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using ThabeSoft.Codecs;
using ThabeSoft.Configuration;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.Triggers;

namespace ThabeSoft.EdgeGateway.Configuration;


public static class ConfigDependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddTransportConfigMapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMapper>()
            where TMapper : class, IConfigMapper<TransportDescriptor>
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<TransportDescriptor>, TMapper>());
            return descriptors;
        }

        public IServiceCollection AddProtocolConfigMapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMapper>()
            where TMapper : class, IConfigMapper<ProtocolDescriptor>
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<ProtocolDescriptor>, TMapper>());
            return descriptors;
        }

        public IServiceCollection AddTriggerConfigMapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMapper>()
            where TMapper : class, IConfigMapper<TriggerDescriptor>
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<TriggerDescriptor>, TMapper>());
            return descriptors;
        }

        public IServiceCollection AddCodecConfigMapper<TDataType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMapper>()
            where TMapper : class, IConfigMapper<CodecDescriptor<TDataType>>
            where TDataType : notnull
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<CodecDescriptor<TDataType>>, TMapper>());
            return descriptors;
        }


        public IServiceCollection AddDataAddressMapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMapper>()
           where TMapper : class, IConfigMapper<DataAddress>
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<DataAddress>, TMapper>());
            return descriptors;
        }
    }
}
