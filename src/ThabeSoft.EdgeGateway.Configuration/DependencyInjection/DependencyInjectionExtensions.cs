using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Configuration.Json;
using ThabeSoft.EdgeGateway.Configuration.Mappers;
using ThabeSoft.EdgeGateway.Configuration.Models;
using ThabeSoft.EdgeGateway.Configuration.Nodes;
using ThabeSoft.EdgeGateway.Configuration.Paths;
using ThabeSoft.EdgeGateway.Configuration.Paths;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Configuration.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGatewayConfiguration()
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigPathParser, ConfigPathParser>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigPathEvaluator, ConfigPathEvaluator>());

            return descriptors;
        }

        public IServiceCollection AddEdgeGatewayJsonConfigurationProvider()
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigSerializer, JsonConfigSerializer>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigNodeParser, JsonConfigNodeParser>());

            return descriptors;
        }


        public IServiceCollection AddAddEdgeGatewayConifigMappers()
        {
            // Gateway
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<GatewayConfig>, GatewayConfigMapper>());
            // Retry
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<RetryDescriptor>, RetryDescriptorConfigMapper>());

            return descriptors;
        }
    }
}