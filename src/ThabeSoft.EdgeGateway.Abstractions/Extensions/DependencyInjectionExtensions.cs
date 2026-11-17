using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Extensions;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddConfigMapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMapper, TValue>()
            where TMapper : class, IConfigMapper<TValue>
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<TValue>, TMapper>());
            return descriptors;
        }

        public IServiceCollection AddTransportDescriptorConfigMapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMapper>()
            where TMapper : class, IConfigMapper<TransportDescriptor>
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<TransportDescriptor>, TMapper>());
            return descriptors;
        }
    }
}
