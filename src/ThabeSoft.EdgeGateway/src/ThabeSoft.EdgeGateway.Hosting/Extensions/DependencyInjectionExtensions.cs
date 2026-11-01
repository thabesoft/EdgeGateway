using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.EdgeGateway.Configs;
using ThabeSoft.EdgeGateway.Services;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGatewayHost(GatewayConfig config)
        {
            descriptors.AddSingleton(config);
            descriptors.AddHostedService<GatewayHostedService>();

            return descriptors;
        }

        public IServiceCollection AddEdgeGatewayHost() =>
            descriptors.AddEdgeGatewayHost(new GatewayConfig() { Ports = [] });
    }
}
