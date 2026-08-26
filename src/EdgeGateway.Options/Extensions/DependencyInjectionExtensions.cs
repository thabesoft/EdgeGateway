using EdgeGateway.Options.Loader;
using EdgeGateway.Options.Monitor;
using EdgeGateway.Options.Serialization;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddEdgeGatewayOptions()
        {
            services.AddSingleton<IGatewayOptionsSerializer, JsonGatewayOptionsSerializer>();
            services.AddSingleton<IGatewayOptionsLoader, GatewayOptionsLoader>();
            services.AddSingleton<IGatewayOptionsMonitor, GatewayOptionsMonitor>();
        }
    }
}
