using EdgeGateway.Configs;
using EdgeGateway.Port;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGateway()
        {
            descriptors.AddSingleton<IPortFactory, PortFactory>();
            descriptors.AddSingleton<IGateway, Gateway>();

            return descriptors;
        }


        public IServiceCollection AddEdgeGateway(GatewayConfig config)
        {
            descriptors.AddSingleton<IPortFactory, PortFactory>();
            descriptors.AddSingleton<IGateway>(sp =>
            {
                var gateway = new Gateway(sp.GetRequiredService<IPortFactory>());

                foreach (var portConfig in config.Ports)
                {
                    var port = gateway.AddPort(portConfig.Descriptor);
                    foreach (var deviceConfig in portConfig.Devices)
                        _ = port.AddDevice(deviceConfig.Descriptor);
                }

                return gateway;
            });

            return descriptors;
        }

        public IServiceCollection AddEdgeGateway(Action<GatewayConfigBuilder> configAction)
        {
            var builder = new GatewayConfigBuilder();
            configAction.Invoke(builder);

            return descriptors.AddEdgeGateway(builder.Build());
        }
    }
}
