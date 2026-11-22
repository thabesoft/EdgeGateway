using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.DependencyInjection;

namespace ThabeSoft.EdgeGateway.Hosting;


public static class HostBuilderExtensions
{
    extension(IHostBuilder hostBuilder)
    {
        /// <summary>
        /// Event, Transport, Protocol, Readings, Subscriptions, Devices, Ports, Host
        /// </summary>
        public IHostBuilder UseEdgeGateway(GatewayDescriptor? descriptor = null)
        {
            return hostBuilder.ConfigureServices((_, descriptors) =>
            {
                descriptors.AddEdgeGatewayReadings();
                descriptors.AddEdgeGatewaySubscriptions();
                descriptors.AddEdgeGatewayDevices();
                descriptors.AddEdgeGatewayPorts();
                descriptors.AddEdgeGateway(descriptor ?? new());

                descriptors.AddEdgeGatewayConfig();
                descriptors.AddEdgeGatewayHost();
            });
        }
    }
}