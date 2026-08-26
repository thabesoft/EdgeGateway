using EdgeGateway.Ethernet;
using EdgeGateway.Options.Loader;
using EdgeGateway.Ports;
using EdgeGateway.SerialPort;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddEdgeGatewaySerialPort()
        {
            // 实例工厂
            services.AddSingleton<IPortFactory, SerialPortFactory>();

            // 配置转换器
            services.AddSingleton<IPortOptionsConverter, SerialPortOptionsConverter>();
        }
    }
}
