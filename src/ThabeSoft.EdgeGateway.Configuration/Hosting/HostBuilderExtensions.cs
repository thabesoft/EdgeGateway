using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.Configuration.DependencyInjection;

namespace ThabeSoft.EdgeGateway.Configuration.Hosting;

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