using Microsoft.Extensions.Hosting;
using ThabeSoft.Configuration.Hosting;
using ThabeSoft.EdgeGateway.App.Hosting;
using ThabeSoft.Codecs.DependencyInjection;
using ThabeSoft.Transports.Ethernet.DependencyInjection;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.DependencyInjection;
using ThabeSoft.EdgeGateway.Modbus.DependencyInjection;
using ThabeSoft.Transports.Serial.DependencyInjection;
using ThabeSoft.Triggers.DependencyInjection;
using System.ComponentModel.Design;

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
            .UseJsonConfig("Gateway:Config")
            .UseEdgeGatewayApp()
            .ConfigureServices(descriptors =>
            {
                descriptors.AddLifecycles();

                descriptors.AddTriggers();
                descriptors.AddTriggersConfig();

                descriptors.AddCodecs();
                descriptors.AddCodecsConfig();

                descriptors.AddSerialTransport();
                descriptors.AddSerialTransportConfig();

                descriptors.AddEthernetTransport();
                descriptors.AddEthernetTransportConfig();

                descriptors.AddEdgeGatewayModbus();
            })
            .Build();
    }
}
