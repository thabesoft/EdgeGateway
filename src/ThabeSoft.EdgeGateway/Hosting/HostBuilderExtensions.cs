using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.DependencyInjection;

namespace ThabeSoft.EdgeGateway.Hosting;


public static class HostBuilderExtensions
{
    extension(IHostBuilder builder)
    {
        public IHostBuilder UseEdgeGateway(GatewayCreationArgs creationContext)
        {
            return builder.ConfigureServices((_, descriptors) =>
            {
                descriptors.AddEdgeGatewayEvents();
                descriptors.AddEdgeGatewayTransport();
                descriptors.AddEdgeGatewayProtocol();
                descriptors.AddEdgeGatewayReadings();

                descriptors.AddEdgeGatewaySubscriptions();
                descriptors.AddEdgeGatewayDevices();
                descriptors.AddEdgeGatewayPorts();

                descriptors.AddEdgeGateway(creationContext);
                descriptors.AddEdgeGatewayHost();
            });
        }
    }
}