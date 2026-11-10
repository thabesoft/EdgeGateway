using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Triggers;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGateway()
        {
            // Trigger
            descriptors.TryAddSingleton(TimeProvider.System);
            descriptors.TryAddSingleton<ITriggerFactory, FixedIntervalTriggerFactory>();
            descriptors.TryAddSingleton<ITriggerFactory, SignalTriggerFactory>();

            // Factory
            descriptors.TryAddSingleton<PortFactory>();
            descriptors.TryAddSingleton<DeviceFactory>();
            descriptors.TryAddSingleton<SubscriptionFactory>();
            descriptors.TryAddSingleton<SubscriptionValueReaderFactory>();

            // FactorySelector
            descriptors.TryAddSingleton<TransportConnectionFactorySelector>();
            descriptors.TryAddSingleton<ProtocolAccessorFactorySelector>();
            descriptors.TryAddSingleton<ITriggerFactorySelector, TriggerFactorySelector>();

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