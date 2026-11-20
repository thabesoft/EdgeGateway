using Microsoft.Extensions.Hosting;
using ThabeSoft.Configuration.DependencyInjection;

namespace ThabeSoft.Configuration.Hosting;

public static class HostBuilderExtensions
{
    extension(IHostBuilder builder)
    {
        public IHostBuilder UseJsonConfig(string optionSectionName)
        {
            return builder.ConfigureServices((_, descriptors) =>
            {
                descriptors.AddEdgeGatewayConfigPaths();
                descriptors.AddEdgeGatewayJsonConfigProvider();

                descriptors.AddEdgeGatewayConfigurationReloader(optionSectionName);
            });
        }
    }
}