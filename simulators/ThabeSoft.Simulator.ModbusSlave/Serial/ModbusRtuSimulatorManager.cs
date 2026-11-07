using NModbus;

namespace ThabeSoft.Simulator.Serial;

public sealed class ModbusRtuSimulatorManager(IModbusFactory modbusFactory, IModbusSlaveNetwork modbusSlaveNetwork) : IModbusSimulatorManager
{
    public IModbusSimulator AddSlave(byte unitId)
    {
        var slave = modbusFactory.CreateSlave(unitId);
        modbusSlaveNetwork.AddSlave(slave);

        return new ModbusSimulator(unitId, slave.DataStore);
    }
}