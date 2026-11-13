using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.EdgeGateway.Simulator.Configs;
using ThabeSoft.EdgeGateway.Simulator.Options;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加视图模块
        /// </summary>
        public IServiceCollection AddApp()
        {
            // Services
            descriptors.AddOptions<SimulatorOptions>()
                .BindConfiguration("Simulator");

            descriptors.AddSingleton<ConfigScriptRunner>();
            descriptors.AddHostedService<ConfigReloadListener>();

            return descriptors;
        }
    }
}