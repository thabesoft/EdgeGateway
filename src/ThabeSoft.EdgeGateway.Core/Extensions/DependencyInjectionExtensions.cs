using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.EdgeGateway.Options;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGateway(Action<IGatewayOptionsBuilder>? buildAction = null)
        {
            // Options
            if (buildAction is not null)
                descriptors.AddGatewayOptions(buildAction);

            // Core
            descriptors.AddSingleton<PortFactory>();
            descriptors.AddSingleton<TransportFactory>();
            descriptors.AddSingleton<DeviceFactory>();
            descriptors.AddSingleton<TagAccessorFactory>();


            descriptors.AddSingleton<IGateway, Gateway>();
            descriptors.AddHostedService<GatewayHostedService>();

            return descriptors;
        }


        /// <summary>
        /// 添加网关配置
        /// </summary>
        public IServiceCollection AddGatewayOptions(Action<IGatewayOptionsBuilder> buildAction)
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


    private delegate void GatewayOptionsHandler(IGatewayOptionsBuilder optionAction);
}