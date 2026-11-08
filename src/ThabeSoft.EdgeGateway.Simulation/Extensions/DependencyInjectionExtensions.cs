using Microsoft.Extensions.DependencyInjection.Extensions;
using NModbus;
using ThabeSoft.EdgeGateway.Simulation;
using ThabeSoft.EdgeGateway.Simulation.Configuration;
using ThabeSoft.EdgeGateway.Simulation.Hosting;
using ThabeSoft.EdgeGateway.Simulation.Networks;
using ThabeSoft.EdgeGateway.Simulation.Networks.Slaves;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddSimulation()
        {
            descriptors.TryAddSingleton<IModbusFactory, ModbusFactory>();
            descriptors.TryAddSingleton<ModbusSimulationNetworkFactory>();
            descriptors.TryAddSingleton<ModbusSimulatedSlaveFactory>();

            descriptors.AddSingleton<ISimulatorHost, SimulatorHost>();
            descriptors.AddHostedService<LaunchHostedService>();

            return descriptors;
        }
    }

    public delegate void ModbusSimulatorConfiguratorHandler(ISimulationConfigurator configurator);
}