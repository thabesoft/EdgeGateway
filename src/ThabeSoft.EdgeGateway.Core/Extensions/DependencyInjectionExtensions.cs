using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGateway()
        {
            descriptors.AddSingleton<IPortFactory, PortFactory>();
            descriptors.AddSingleton<IDeviceFactory, DefaultDeviceFactory>();
            return descriptors.AddSingleton<IGateway, Gateway>();
        }
    }
}
