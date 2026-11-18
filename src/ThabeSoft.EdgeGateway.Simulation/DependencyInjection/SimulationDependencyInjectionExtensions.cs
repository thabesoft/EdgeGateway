using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NModbus;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Devices.Modbus;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Ports.Modbus;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.DependencyInjection;


public static class SimulationDependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加模拟模块
        /// </summary>
        public IServiceCollection AddEdgeGatewaySimulation(SimulatorCreationArgs args)
        {
            descriptors.TryAddSingleton<SimulatorFactory>();
            descriptors.TryAddSingleton<ISimulator>(sp =>
            {
                var factory = sp.GetRequiredService<SimulatorFactory>();
                return factory.Create(args);
            });

            return descriptors;
        }

        public IServiceCollection AddEdgeGatewaySimulationDevice()
        {
            descriptors.TryAddSingleton<TriggerManager>();
            descriptors.TryAddSingleton<SimulationPortFactory>();
            descriptors.TryAddSingleton<SimulationDeviceFactory>();

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
            descriptors.TryAddSingleton<ISimulationDeviceResolver, ModbusSimulationDeviceResolver>();
            descriptors.TryAddSingleton<ISimulationPortResolver, ModbusSimulationPortResolver>();
            return descriptors;
        }
    }
}