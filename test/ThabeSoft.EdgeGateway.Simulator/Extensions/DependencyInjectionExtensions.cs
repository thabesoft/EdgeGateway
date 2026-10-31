using Microsoft.Extensions.DependencyInjection;
using EdgeGateway.Demo.Options;
using ThabeSoft.EdgeGateway.Simulator.Services;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddSerialSimulator(Action<SerialPortSlaveHostedServiceOptions> optionsAction)
        {
            descriptors.Configure(optionsAction);
            descriptors.AddHostedService<SerialPortSlaveHostedService>();

            return descriptors;
        }
    }
}
