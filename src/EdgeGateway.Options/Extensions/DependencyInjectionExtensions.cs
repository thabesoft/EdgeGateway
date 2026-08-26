using EdgeGateway.Loader;
using EdgeGateway.Serialization;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddEdgeGatewayJsonOptions(Action<GatewaySourceOptions>? optionsAction = null)
        {
            if (optionsAction is not null) services.Configure(optionsAction);

            services.AddSingleton<IGatewayOptionsSerializer, JsonGatewayOptionsSerializer>();
            services.AddSingleton<IGatewayOptionsLoader, GatewayOptionsLoader>();
        }
    }
}
