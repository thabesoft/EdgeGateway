using EdgeGateway;
using EdgeGateway.Devices;
using EdgeGateway.Ports;
using EdgeGateway.Tags;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddEdgeGateway()
        {
            services.AddScoped<IDeviceManager, DeviceManager>();
            services.AddScoped<IPortManager, PortManager>();

            services.AddScoped<ITagReader, TagReader>();
            services.AddScoped<ITagWriter, TagWriter>();

            services.AddScoped<IGateway, Gateway>();
            //services.AddScoped<IGateway, Gateway>();
        }
    }
}
