using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transport;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGateway()
        {
            // Events
            descriptors.TryAddSingleton<IEventPublisher, EventPublisher>();

            // Triggers
            descriptors.AddTriggers();

            // Factory
            descriptors.TryAddSingleton<PortFactory>();
            descriptors.TryAddSingleton<DeviceFactory>();
            descriptors.TryAddSingleton<SubscriptionFactory>();

            // FactorySelector
            descriptors.TryAddSingleton<TransportConnectionFactorySelector>();
            descriptors.TryAddSingleton<ProtocolAccessorFactorySelector>();

            // Gateway
            descriptors.TryAddSingleton<IGateway, Gateway>();

            return descriptors;
        }

        public IServiceCollection AddEdgeGatewayHost()
        {
            return descriptors.AddHostedService<LaunchHostedService>();
        }
    }
}