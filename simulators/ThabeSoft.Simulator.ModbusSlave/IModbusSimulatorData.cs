namespace ThabeSoft.Simulator;

public interface IModbusSimulatorData
{
    void SetCoils(ushort address, params bool[] values);
    void SetHoldingRegisters(ushort address, params ushort[] values);
}
