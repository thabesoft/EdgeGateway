using Microsoft.Extensions.Logging;
using NModbus;
using ThabeSoft.EdgeGateway.Simulation.Networks.Slaves;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Simulation;


internal sealed class ModbusSimulatedSlaveFactory(
        IModbusFactory modbusFactory,
        ITriggerFactorySelector triggerFactorySelector,
        ILoggerFactory loggerFactory)
{
    public ModbusSimulationSlave Create(byte unitId) => new
    (
        unitId: unitId,
        modbusFactory: modbusFactory,
        triggerFactorySelector: triggerFactorySelector,
        logger: loggerFactory.CreateLogger<ModbusSimulationSlave>()
    );
}
