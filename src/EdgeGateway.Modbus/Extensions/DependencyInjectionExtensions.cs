using EdgeGateway.Devices;
using EdgeGateway.Modbus;
using Microsoft.Extensions.DependencyInjection.Extensions;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddEdgeGatewayModbus()
        {
            services.TryAddSingleton<IDeviceFactory, ModbusRtuDeviceFactory>();
            services.TryAddSingleton<IDeviceFactory, ModbusAsciiDeviceFactory>();
            services.TryAddSingleton<IDeviceFactory, ModbusIpDeviceFactory>();
        }
    }
}
