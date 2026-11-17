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

            builder.ConfigureServices(services =>
            {
                services.AddEdgeGateway();
                services.AddEdgeGatewaySerial();
                services.AddEdgeGatewayEthernet();
                services.AddEdgeGatewayModbus();

                // Host
                services.AddEdgeGatewayHost();

                // APP
                services.AddEdgeGatewayApp();
                services.AddEdgeGatewayAppHost();

                // 用配置初始化
                services.AddEdgeGatewayJsonConfiguration();
            });

            return builder;
        }
    }
}
