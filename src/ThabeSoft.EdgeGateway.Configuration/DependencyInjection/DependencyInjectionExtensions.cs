using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Configuration.Json;

namespace ThabeSoft.EdgeGateway.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGatewayJsonConfiguration()
        {
            descriptors.TryAddSingleton(_ => new ConfigFileWatcher("Gateway.json", TimeSpan.FromSeconds(1)));
            descriptors.TryAddSingleton<ConfigManager>();
            descriptors.AddHostedService<JsonFileConfigHostedService>();
            descriptors.TryAddSingleton<IConfigSerializer, JsonConfigSerializer>();

            return descriptors;
        }
    }
}