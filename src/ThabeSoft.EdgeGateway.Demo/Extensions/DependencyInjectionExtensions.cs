using EdgeGateway.Options;
using EdgeGateway.Services;
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
            // 启动从站
            descriptors.Configure<SerialPortSlaveHostedServiceOptions>(x => x.PortName = "COM3");
            descriptors.AddHostedService<SerialPortSlaveHostedService>();


            // 启动网关
            return descriptors.AddHostedService<GatewayHostedService>();
        }
    }
}
