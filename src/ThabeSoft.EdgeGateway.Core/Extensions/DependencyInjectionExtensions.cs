using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Triggers;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGateway(Action<IGatewayOptionsConfigurator>? buildAction = null)
        {
            // Options
            descriptors.ConfigureGateway(buildAction ?? delegate { });

            // Manager
            descriptors.AddSingleton<TransportManager>();
            descriptors.AddSingleton<TriggerManager>();
            descriptors.AddSingleton<ProtocolAccessorManager>();
            descriptors.AddSingleton<SubscriptionManager>();
            descriptors.AddSingleton<DeviceManager>();
            descriptors.AddSingleton<PortManageer>();

            // Trigger
            descriptors.TryAddSingleton(TimeProvider.System);
            descriptors.AddSingleton<ITriggerFactory, FixedIntervalTriggerFactory>();
            descriptors.AddSingleton<ITriggerFactory, SignalTriggerFactory>();

            // Gateway
            descriptors.AddSingleton<IGateway, Gateway>();
            descriptors.AddHostedService<GatewayHostedService>();

            return descriptors;
        }


        /// <summary>
        /// 添加网关配置
        /// </summary>
        public IServiceCollection ConfigureGateway(Action<IGatewayOptionsConfigurator> buildAction)
        {
            // 配置实例
            descriptors.TryAddSingleton<GatewayOptions>(sp =>
            {
                var builder = new GatewayOptionsBuilder();

                foreach (var action in sp.GetServices<GatewayOptionsHandler>())
                    action.Invoke(builder);

                return builder.Build();
            });

            // 配置过程
            if (buildAction is null) return descriptors;
            return descriptors.AddSingleton(new GatewayOptionsHandler(buildAction));
        }
    }


    private delegate void GatewayOptionsHandler(IGatewayOptionsConfigurator optionAction);
}