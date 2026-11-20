using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using ThabeSoft.Configuration;

namespace ThabeSoft.EdgeGateway.Transports;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddTransportConfigMapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMapper>()
            where TMapper : class, IConfigMapper<TransportDescriptor>
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<TransportDescriptor>, TMapper>());
            return descriptors;
        }
    }
}
