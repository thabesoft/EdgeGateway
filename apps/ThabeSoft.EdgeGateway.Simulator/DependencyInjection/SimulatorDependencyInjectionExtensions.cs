using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.EdgeGateway.Simulator.Configs;
using ThabeSoft.EdgeGateway.Simulator.Options;

namespace ThabeSoft.EdgeGateway.Simulator.DependencyInjection;


public static class SimulatorDependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加视图模块
        /// </summary>
        public IServiceCollection AddEdgeGatewaySimulatorApp()
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