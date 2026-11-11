using Microsoft.Extensions.Logging;
using NModbus;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Devices.Modbus;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Slaves.Modbus;


internal sealed class ModbusSimulatedSlaveFactory(
        IModbusFactory modbusFactory,
        ITriggerFactorySelector triggerFactorySelector,
        ILoggerFactory loggerFactory
    ) : ISimulationDeviceFactory
{
    public bool CanCreate(SimulationDeviceDescriptor descriptor)
    {
        return descriptor.Id is ModbusSimulationDeviceId;
    }

    public ISimulationDevice Create(SimulationDeviceDescriptor descriptor)
    {
        if (descriptor.Id is not ModbusSimulationDeviceId modbus)
            throw new InvalidOperationException("不支持的从站Id");

        return new ModbusSimulationDevice
        (
            modbus,
            modbusFactory: modbusFactory,
            triggerFactorySelector: triggerFactorySelector,
            logger: loggerFactory.CreateLogger<ModbusSimulationDevice>()
        );
    }
}
