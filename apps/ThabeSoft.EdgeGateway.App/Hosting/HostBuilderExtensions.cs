using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.App.DependencyInjection;
using ThabeSoft.EdgeGateway.DependencyInjection;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Hosting;

namespace ThabeSoft.EdgeGateway.App.Hosting;


internal static class HostBuilderExtensions
{
    extension(IHostBuilder builder)
    {
        public IHostBuilder UseEdgeGatewayApp()
        {
            builder.UseEdgeGateway(x =>
            {
                x.WithGatewayStateMachine(Lifecycle.RetryableStateMachine(x => x
                    .WithStartTimeout(TimeSpan.FromSeconds(120))
                    .WithStopTimeout(TimeSpan.FromSeconds(60))
                    .WithDisposeTimeout(TimeSpan.FromSeconds(30))
                    .WithExecuteTimeout(TimeSpan.FromSeconds(30))));

                x.WithPortStateMachine(Lifecycle.RetryableStateMachine(x => x
                    .WithStartTimeout(TimeSpan.FromSeconds(60))
                    .WithStopTimeout(TimeSpan.FromSeconds(30))
                    .WithDisposeTimeout(TimeSpan.FromSeconds(15))
                    .WithExecuteTimeout(TimeSpan.FromSeconds(30))));

                x.WithTransportStateMachine(Lifecycle.RetryableStateMachine(x => x
                    .WithStartTimeout(TimeSpan.FromSeconds(60))
                    .WithStopTimeout(TimeSpan.FromSeconds(30))
                    .WithDisposeTimeout(TimeSpan.FromSeconds(15))
                    .WithExecuteTimeout(TimeSpan.FromSeconds(30))));

                x.WithDeviceStateMachine(Lifecycle.RetryableStateMachine(x => x
                    .WithStartTimeout(TimeSpan.FromSeconds(30))
                    .WithStopTimeout(TimeSpan.FromSeconds(15))
                    .WithDisposeTimeout(TimeSpan.FromSeconds(10))
                    .WithExecuteTimeout(TimeSpan.FromSeconds(30))));

                x.WithSubscriptionStateMachine(Lifecycle.RetryableStateMachine(x => x
                    .WithStartTimeout(TimeSpan.FromSeconds(15))
                    .WithStopTimeout(TimeSpan.FromSeconds(10))
                    .WithDisposeTimeout(TimeSpan.FromSeconds(5))
                    .WithExecuteTimeout(TimeSpan.FromSeconds(30))));
            });

            builder.ConfigureServices(descriptors =>
            {
                descriptors.AddEdgeGatewayTriggers();
                descriptors.AddEdgeGatewayLifecycles();

                descriptors.AddEdgeGatewaySerial();
                descriptors.AddEdgeGatewayEthernet();
                descriptors.AddEdgeGatewayModbus();

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
