using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.EdgeGateway.Demo.Services;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddDemo()
        {
            // 添加串口模拟器
            descriptors.AddModbusRtuSimulator(x => x.PortName = "COM3");
            descriptors.AddHostedService<ModbusSimulatorHostedService>();


            // 启动网关
            return descriptors.AddHostedService<GatewayHostedService>();
        }
    }
}
