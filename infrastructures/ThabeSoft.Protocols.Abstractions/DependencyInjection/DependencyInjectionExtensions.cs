using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using ThabeSoft.Configuration;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.Protocols;

namespace ThabeSoft.EdgeGateway.Configuration;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddProtocolConfigMapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMapper>()
            where TMapper : class, IConfigMapper<ProtocolDescriptor>
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<ProtocolDescriptor>, TMapper>());
            return descriptors;
        }

        public IServiceCollection AddDataAddressMapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMapper>()
           where TMapper : class, IConfigMapper<ProtocolAddress>
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<ProtocolAddress>, TMapper>());
            return descriptors;
        }
    }
}
