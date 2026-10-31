using ThabeSoft.EdgeGateway.Tag;

namespace ThabeSoft.EdgeGateway.Modbus;


public sealed record class ModbusDataAddress : IDataAddress
{
    public required byte SlaveId { get; init; }
    public required ModbusDataKind DataKind { get; init; }
    public required ushort StartAddress { get; init; }
    public required ushort Quantity { get; init; }


    private ModbusDataAddress() { }

    public bool Equals(IDataAddress? other) =>
        Equals(other as ModbusDataAddress);


    public static ModbusDataAddress Coil(byte slaveId, ushort address, ushort quantity)
    {
        return new()
        {
            SlaveId = slaveId,
            DataKind = ModbusDataKind.Coil,
            StartAddress = address,
            Quantity = quantity
        };
    }
    public static ModbusDataAddress DiscreteInput(byte slaveId, ushort address, ushort quantity)
    {
        return new()
        {
            SlaveId = slaveId,
            DataKind = ModbusDataKind.DiscreteInput,
            StartAddress = address,
            Quantity = quantity
        };
    }
    public static ModbusDataAddress HoldingRegister(byte slaveId, ushort address, ushort quantity)
    {
        return new()
        {
            SlaveId = slaveId,
            DataKind = ModbusDataKind.HoldingRegister,
            StartAddress = address,
            Quantity = quantity
        };
    }
    public static ModbusDataAddress InputRegister(byte slaveId, ushort address, ushort quantity)
    {
        return new()
        {
            SlaveId = slaveId,
            DataKind = ModbusDataKind.InputRegister,
            StartAddress = address,
            Quantity = quantity
        };
    }
}
