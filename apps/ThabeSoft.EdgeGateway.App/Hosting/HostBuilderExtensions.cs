using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.App.DependencyInjection;

namespace ThabeSoft.EdgeGateway.App.Hosting;


public static class HostBuilderExtensions
{
    extension(IHostBuilder hostBuilder)
    {
        public IHostBuilder UseEdgeGatewayApp()
        {
            return hostBuilder.ConfigureServices((_, descriptors) =>
            {
                descriptors.AddEdgeGatewayApp();
                descriptors.AddEdgeGatewayAppHost();
            });
        }
    }
}