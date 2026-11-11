using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NModbus;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Ports.Modbus;
using ThabeSoft.EdgeGateway.Slaves.Modbus;

namespace ThabeSoft.EdgeGateway;


public static class SimulationDependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddSimulation()
        {
            descriptors.TryAddSingleton<SimulationDeviceFactorySelector>();
            descriptors.TryAddSingleton<SimulationPortFactorySelector>();
            descriptors.TryAddSingleton<ISimulator, Simulator>();
            return descriptors;
        }
        public IServiceCollection AddSimulationHost()
        {
            descriptors.AddHostedService<LaunchHostedService>();
            return descriptors;
        }


        public IServiceCollection AddModbusSimulation()
        {
            descriptors.TryAddSingleton<IModbusFactory, ModbusFactory>();
            descriptors.TryAddSingleton<ISimulationDeviceFactory, ModbusSimulatedSlaveFactory>();
            descriptors.TryAddSingleton<ISimulationPortFactory, ModbusSimulationPortFactory>();
            return descriptors;
        }
    }
}