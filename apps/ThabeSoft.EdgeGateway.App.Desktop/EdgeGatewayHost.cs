using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.App.DependencyInjection;
using ThabeSoft.EdgeGateway.App.Desktop.DependencyInjection;
using ThabeSoft.EdgeGateway.Codecs.DependencyInjection;
using ThabeSoft.EdgeGateway.Configuration.DependencyInjection;
using ThabeSoft.EdgeGateway.DependencyInjection;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.DependencyInjection;
using ThabeSoft.EdgeGateway.Modbus.DependencyInjection;
using ThabeSoft.EdgeGateway.Serial.DependencyInjection;
using ThabeSoft.EdgeGateway.Triggers.DependencyInjection;

namespace ThabeSoft.EdgeGateway.App.Desktop;


internal static class EdgeGatewayHost
{
    public static IHost Create()
    {
        return Host.CreateDefaultBuilder()
            .UseEdgeGateway(x =>
            {
                x.WithGatewayStateMachine(LifecycleBuilder.RetryableStateMachine(x => x
                    .WithStartTimeout(TimeSpan.FromSeconds(120))
                    .WithStopTimeout(TimeSpan.FromSeconds(60))
                    .WithDisposeTimeout(TimeSpan.FromSeconds(30))
                    .WithExecuteTimeout(TimeSpan.FromSeconds(30))));

                x.WithPortStateMachine(LifecycleBuilder.RetryableStateMachine(x => x
                    .WithStartTimeout(TimeSpan.FromSeconds(60))
                    .WithStopTimeout(TimeSpan.FromSeconds(30))
                    .WithDisposeTimeout(TimeSpan.FromSeconds(15))
                    .WithExecuteTimeout(TimeSpan.FromSeconds(30))));

                x.WithTransportStateMachine(LifecycleBuilder.RetryableStateMachine(x => x
                    .WithStartTimeout(TimeSpan.FromSeconds(60))
                    .WithStopTimeout(TimeSpan.FromSeconds(30))
                    .WithDisposeTimeout(TimeSpan.FromSeconds(15))
                    .WithExecuteTimeout(TimeSpan.FromSeconds(30))));

                x.WithDeviceStateMachine(LifecycleBuilder.RetryableStateMachine(x => x
                    .WithStartTimeout(TimeSpan.FromSeconds(30))
                    .WithStopTimeout(TimeSpan.FromSeconds(15))
                    .WithDisposeTimeout(TimeSpan.FromSeconds(10))
                    .WithExecuteTimeout(TimeSpan.FromSeconds(30))));

                x.WithSubscriptionStateMachine(LifecycleBuilder.RetryableStateMachine(x => x
                    .WithStartTimeout(TimeSpan.FromSeconds(15))
                    .WithStopTimeout(TimeSpan.FromSeconds(10))
                    .WithDisposeTimeout(TimeSpan.FromSeconds(5))
                    .WithExecuteTimeout(TimeSpan.FromSeconds(30))));
            })
            .ConfigureServices(descriptors =>
            {
                descriptors.AddEdgeGatewayTriggers();
                descriptors.AddEdgeGatewayLifecycles();
                descriptors.AddEdgeGatewayCodec();

                descriptors.AddEdgeGatewaySerial();
                descriptors.AddEdgeGatewayEthernet();
                descriptors.AddEdgeGatewayModbus();

                // APP
                descriptors.AddEdgeGatewayApp();
                descriptors.AddEdgeGatewayAppHost();
                descriptors.AddEdgeGatewayAppDesktop();

                // 用配置初始化
                descriptors.AddEdgeGatewayJsonConfiguration();
            })
            .Build();
    }
}
