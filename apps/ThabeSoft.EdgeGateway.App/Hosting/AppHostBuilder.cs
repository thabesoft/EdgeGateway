using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.App.DependencyInjection;
using ThabeSoft.EdgeGateway.DependencyInjection;

namespace ThabeSoft.EdgeGateway.App.Hosting;

internal static class AppHostBuilder
{
    extension(IHostBuilder builder)
    {
        public IHostBuilder UseEdgeGatewayApp()
        {
            builder.ConfigureLogging((_, builder) => builder
                .SetMinimumLevel(LogLevel.Information));

            builder.ConfigureServices(descriptors =>
            {
                descriptors.AddEdgeGateway();
                descriptors.AddEdgeGatewayTriggers();
                descriptors.AddEdgeGatewaySerial();
                descriptors.AddEdgeGatewayEthernet();
                descriptors.AddEdgeGatewayModbus();

                // Host
                descriptors.AddEdgeGatewayHost();

                // APP
                descriptors.AddEdgeGatewayApp();
                descriptors.AddEdgeGatewayAppHost();

                // 用配置初始化
                descriptors.AddEdgeGatewayJsonConfiguration();
            });

            return builder;
        }
    }
}
