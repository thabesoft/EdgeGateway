using ThabeSoft.EdgeGateway.Configs;
using ThabeSoft.EdgeGateway.Device;
using ThabeSoft.EdgeGateway.Port;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        private void AddServices()
        {
            descriptors.AddSingleton<IPortFactory, PortFactory>();
            descriptors.AddSingleton<IDeviceFactory, DefaultDeviceFactory>();
        }


        public IServiceCollection AddEdgeGateway()
        {
            descriptors.AddServices();
            return descriptors.AddSingleton<IGateway, Gateway>();
        }


        public IServiceCollection AddEdgeGateway(GatewayConfig config)
        {
            descriptors.AddServices();
            return descriptors.AddSingleton<IGateway>(sp =>
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
        }

        public IServiceCollection AddEdgeGateway(Action<GatewayConfigBuilder> configAction)
        {
            descriptors.AddServices();

            var builder = new GatewayConfigBuilder();
            configAction.Invoke(builder);
            return descriptors.AddEdgeGateway(builder.Build());
        }
    }
}
