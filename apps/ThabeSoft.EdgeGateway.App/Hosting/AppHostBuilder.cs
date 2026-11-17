using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.App.DependencyInjection;
using ThabeSoft.EdgeGateway.DependencyInjection;
using ThabeSoft.EdgeGateway.Hosting;

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
                descriptors.AddEdgeGatewayTriggers();
                descriptors.AddEdgeGatewayLifecycles();

                descriptors.AddEdgeGateway(new()
                {
                    GatewayStateMachine = Lifecycle.RetryableStateMachine(),
                    PortStateMachine = Lifecycle.RetryableStateMachine(),
                    TransportStateMachine = Lifecycle.StrictStateMachine(),
                    DeviceStateMachine = Lifecycle.RetryableStateMachine(),
                    SubscriptionStateMachine = Lifecycle.StrictStateMachine()
                });

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
