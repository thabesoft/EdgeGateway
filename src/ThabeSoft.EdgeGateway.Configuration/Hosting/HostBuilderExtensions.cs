using Microsoft.Extensions.Hosting;
using ThabeSoft.Configuration.DependencyInjection;

namespace ThabeSoft.Configuration.Hosting;

public static class HostBuilderExtensions
{
    extension(IHostBuilder builder)
    {
        public IHostBuilder UseEdgeGatewayJsonConfig()
        {
            return builder.ConfigureServices((_, descriptors) =>
            {
                descriptors.AddEdgeGatewayConfigPaths();
                descriptors.AddEdgeGatewayConfigMappers();
                descriptors.AddEdgeGatewayJsonConfigProvider();

                descriptors.AddEdgeGatewayConfigurationReloader();
            });
        }
    }
}