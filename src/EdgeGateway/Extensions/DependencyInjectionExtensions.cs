using EdgeGateway;
using EdgeGateway.Devices;
using EdgeGateway.Devices.Modbus;
using EdgeGateway.Options;
using EdgeGateway.Ports;
using EdgeGateway.Ports.Ethernet;
using EdgeGateway.Ports.SerialPort;
using EdgeGateway.Protocols;
using EdgeGateway.Tags;
using Microsoft.Extensions.DependencyInjection.Extensions;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddEdgeGateway()
        {
            services.AddScoped<IDeviceFactoryManager, DeviceFactoryManager>();
            services.AddScoped<IDeviceManager, DeviceManager>();

            services.AddScoped<IPortFactoryManager, PortFactoryManager>();
            services.AddScoped<IPortManager, PortManager>();

            services.AddScoped<ITagReader, TagReader>();
            services.AddScoped<ITagWriter, TagWriter>();

            services.AddScoped<IGateway, Gateway>();

            // 配置
            services.AddSingleton<IPortFactory, SerialPortFactory>();
            services.TryAddSingleton<IPortFactory, EthernetPortFactory>();
            services.TryAddSingleton<IDeviceFactory, ModbusDeviceFactory>();
        }
    }
}
