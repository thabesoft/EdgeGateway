using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.EdgeGateway.Options;
using ThabeSoft.EdgeGateway.Ports;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGateway(Action<GatewayOptionsBuilder>? buildAction = null)
        {
            // Options
            var builder = new GatewayOptionsBuilder();
            buildAction?.Invoke(builder);
            descriptors.AddSingleton(builder.Build());

            // Core
            descriptors.AddSingleton<IPortFactory, PortFactory>();
            descriptors.AddSingleton<IDeviceFactory, DeviceFactory>();

            descriptors.AddSingleton<IGateway, Gateway>();
            descriptors.AddHostedService<GatewayHostedService>();

            return descriptors;
        }
    }
}
