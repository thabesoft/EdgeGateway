namespace ThabeSoft.EdgeGateway.Devices.Modbus;


public sealed class ModbusSimulationDeviceId : ISimulationDeviceId
{
    public byte Value { get; }
    public ModbusSimulationDeviceId(byte id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), id, "从站Id必须大于0");

        Value = id;
    }
    public override string ToString() => $"SlaveId({Value})";


    public bool Equals(ISimulationDeviceId? other)
    {
        if (other is not ModbusSimulationDeviceId id) return false;
        return id.Value == Value;
    }
}
