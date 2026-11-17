using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NModbus;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Devices.Modbus;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Ports.Modbus;

namespace ThabeSoft.EdgeGateway.DependencyInjection;


public static class SimulationDependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加模拟模块
        /// </summary>
        public IServiceCollection AddEdgeGatewaySimulation()
        {
            descriptors.TryAddSingleton<SimulationDeviceFactory>();
            descriptors.TryAddSingleton<SimulationPortFactorySelector>();
            descriptors.TryAddSingleton<TriggerManager>();
            descriptors.TryAddSingleton<ISimulator, Simulator>();
            return descriptors;
        }
        public IServiceCollection AddEdgeGatewaySimulationHost()
        {
            descriptors.AddHostedService<LaunchHostedService>();
            return descriptors;
        }

        /// <summary>
        /// 添加 Modbus 模拟模块
        /// </summary>
        public IServiceCollection AddEdgeGatewayModbusSimulation()
        {
            descriptors.TryAddSingleton<IModbusFactory, ModbusFactory>();
            descriptors.TryAddSingleton<ISimulationDeviceResolver, ModbusSimulationSlaveFactory>();
            descriptors.TryAddSingleton<ISimulationPortFactory, ModbusSimulationPortFactory>();
            return descriptors;
        }
    }
}