using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.EdgeGateway.Options;
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
        public IServiceCollection AddEdgeGateway(Action<GatewayOptions>? optionsAction = null)
        {
            if (optionsAction is not null)
                descriptors.Configure(optionsAction);

            // Manager
            descriptors.TryAddSingleton<TransportManager>();
            descriptors.TryAddSingleton<TriggerManager>();
            descriptors.TryAddSingleton<ProtocolAccessorManager>();
            descriptors.TryAddSingleton<SubscriptionManager>();
            descriptors.TryAddSingleton<DeviceManager>();
            descriptors.TryAddSingleton<PortManageer>();

            // Trigger
            descriptors.TryAddSingleton(TimeProvider.System);
            descriptors.TryAddSingleton<ITriggerFactory, FixedIntervalTriggerFactory>();
            descriptors.TryAddSingleton<ITriggerFactory, SignalTriggerFactory>();

            // Gateway
            descriptors.TryAddSingleton<IGateway, Gateway>();
            descriptors.AddHostedService<ConfiguratorHostedService>();
            descriptors.AddHostedService<LaunchHostedService>();

            return descriptors;
        }

        /// <summary>
        /// 添加网关配置
        /// </summary>
        public IServiceCollection ConfigureGateway(GatewayConfiguratorHandler configuratorHandler)
        {
            return descriptors.AddSingleton(configuratorHandler);
        }
    }
}