using NModbus;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Simulation.Networks.Slaves;


internal sealed class ModbusSimulatedSlaveFactory(IModbusFactory modbusFactory, ITriggerFactorySelector triggerFactorySelector)
{
    public ModbusSimulatedSlave Create(byte unitId) => new
    (
        unitId: unitId,
        modbusFactory: modbusFactory,
        triggerFactorySelector: triggerFactorySelector
    );
}
