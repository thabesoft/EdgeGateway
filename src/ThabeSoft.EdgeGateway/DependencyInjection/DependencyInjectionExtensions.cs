using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.Configuration;
using ThabeSoft.Configuration.Rloader;
using ThabeSoft.EdgeGateway.Configuration.Mappers;
using ThabeSoft.EdgeGateway.Configuration.Models;
using ThabeSoft.EdgeGateway.Devices;
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
        /// <summary>
        /// 提供网关和网关工厂
        /// </summary>
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

        /// <summary>
        /// 提供网关跟随Host启动能力
        /// </summary>
        public IServiceCollection AddEdgeGatewayHost()
        {
            return descriptors.AddHostedService<LaunchHostedService>();
        }

        /// <summary>
        /// 提供网关配置解析映射能力
        /// </summary>
        public IServiceCollection AddEdgeGatewayConfig()
        {
            descriptors.TryAddSingleton<IConfigReloadApplier, GatewayConfigApplier>();
            // Gateway
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<GatewayConfig>, GatewayConfigMapper>());
            // Retry
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<RetryDescriptor>, RetryDescriptorConfigMapper>());

            return descriptors;
        }


        /// <summary>
        /// 提供网关端口管理能力
        /// </summary>
        public IServiceCollection AddEdgeGatewayPorts()
        {
            descriptors.TryAddSingleton<PortFactory>();
            descriptors.TryAddSingleton<PortManagerFactory>();

            return descriptors;
        }
        /// <summary>
        /// 提供网关设备管理能力
        /// </summary>
        public IServiceCollection AddEdgeGatewayDevices()
        {
            descriptors.TryAddSingleton<DeviceFactory>();
            descriptors.TryAddSingleton<DeviceManagerFactory>();

            return descriptors;
        }
        /// <summary>
        /// 提供网关订阅管理能力
        /// </summary>
        public IServiceCollection AddEdgeGatewaySubscriptions()
        {
            descriptors.TryAddSingleton<SubscriptionFactory>();
            descriptors.TryAddSingleton<SubscriptionManagerFactory>();

            return descriptors;
        }
        /// <summary>
        /// 提供网关数据点读取能力
        /// </summary>
        public IServiceCollection AddEdgeGatewayReadings()
        {
            descriptors.TryAddSingleton<ReadingSinkBus>();
            descriptors.TryAddSingleton<IReadingSink, TestReadingSink>();

            return descriptors;
        }
    }
}