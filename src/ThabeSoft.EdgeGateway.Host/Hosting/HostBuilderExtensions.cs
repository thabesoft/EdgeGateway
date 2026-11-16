using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.DependencyInjection;

namespace ThabeSoft.EdgeGateway.Hosting;


public static class HostBuilderExtensions
{
    extension(IHostBuilder builder)
    {
        public IHostBuilder UseEdgeGateway()
        {
            // Services
            builder.ConfigureServices((_, descriptors) =>
            {
                descriptors.AddEdgeGateway();
                descriptors.AddEdgeGatewayEthernet();
                descriptors.AddEdgeGatewayModbus();
                descriptors.AddEdgeGatewaySerial();

            });

            return builder;
        }
    }
}