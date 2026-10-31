using EdgeGateway.Services;
using EdgeGateway.Services.ModbusSlave;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddDemo()
        {
            // 模拟从站
            descriptors.AddHostedService<ModbusSalveBackgroundService>();

            // 启动网关
            return descriptors.AddHostedService<GatewayHostedService>();
        }
    }
}
