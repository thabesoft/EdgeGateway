using Microsoft.Extensions.Logging;
using NModbus;

namespace ThabeSoft.EdgeGateway.Devices.Modbus;


internal sealed class ModbusSimulationSlaveFactory(
        IModbusFactory modbusFactory,
        TriggerManager triggerManager,
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
            triggerManager: triggerManager,
            logger: loggerFactory.CreateLogger<ModbusSimulationDevice>()
        );
    }
}
