using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.Configuration;
using ThabeSoft.Configuration.Rloader;
using ThabeSoft.EdgeGateway.Configuration.Mappers;
using ThabeSoft.EdgeGateway.Configuration.Models;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Readings;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.Transports;

namespace ThabeSoft.EdgeGateway.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGatewayHost()
        {
            return descriptors.AddHostedService<LaunchHostedService>();
        }

        public IServiceCollection AddEdgeGateway(GatewayCreationArgs creationContext)
        {
            descriptors.TryAddSingleton<GatewayFactory>();
            descriptors.TryAddSingleton<IGateway>(sp =>
            {
                var gatewayFactory = sp.GetRequiredService<GatewayFactory>();
                return gatewayFactory.Create(creationContext);
            });
            return descriptors;
        }
        public IServiceCollection AddEdgeGatewayPorts()
        {
            descriptors.TryAddSingleton<PortFactory>();
            descriptors.TryAddSingleton<PortManagerFactory>();

            return descriptors;
        }
        public IServiceCollection AddEdgeGatewayDevices()
        {
            descriptors.TryAddSingleton<DeviceFactory>();
            descriptors.TryAddSingleton<DeviceManagerFactory>();

            return descriptors;
        }
        public IServiceCollection AddEdgeGatewaySubscriptions()
        {
            descriptors.TryAddSingleton<SubscriptionFactory>();
            descriptors.TryAddSingleton<SubscriptionManagerFactory>();

            return descriptors;
        }
        public IServiceCollection AddEdgeGatewayTransport()
        {
            descriptors.TryAddSingleton<ITransportConnectionFactory, TransportConnectionFactory>();
            return descriptors;
        }

        public IServiceCollection AddEdgeGatewayProtocol()
        {
            descriptors.TryAddSingleton<ProtocolAccessorFactory>();
            return descriptors;
        }

        public IServiceCollection AddEdgeGatewayReadings()
        {
            descriptors.TryAddSingleton<ReadingSinkBus>();
            descriptors.TryAddSingleton<IReadingSink, TestReadingSink>();

            return descriptors;
        }


        /// <summary>
        /// 提供配置解析映射能力
        /// </summary>
        public IServiceCollection AddEdgeGatewayConfiguration()
        {
            descriptors.TryAddSingleton<IConfigReloadApplier, GatewayConfigApplier>();

            // Gateway
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<GatewayConfig>, GatewayConfigMapper>());
            // Retry
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<RetryDescriptor>, RetryDescriptorConfigMapper>());

            return descriptors;
        }
    }
}