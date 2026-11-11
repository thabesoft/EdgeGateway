namespace ThabeSoft.EdgeGateway.Devices.Modbus;

public sealed class ModbusSimulationDeviceDescriptor : ISimulationDeviceDescriptor
{
    public ModbusSimulationDeviceDescriptor(ModbusSimulationDeviceId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        Id = id;
    }

    public ModbusSimulationDeviceId Id { get; }
    ISimulationDeviceId ISimulationDeviceDescriptor.Id => Id;


    public override string ToString() => Id.ToString();
}