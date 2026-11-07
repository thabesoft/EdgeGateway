using ThabeSoft.EdgeGateway.Codecs;

namespace ThabeSoft.EdgeGateway.Addressing;

public sealed class ModbusDataAddress : IDataAddress, IRequiredBits
{
    public required byte SlaveId { get; init; }
    public required ModbusDataKind DataKind { get; init; }
    public required ushort StartAddress { get; init; }
    public required ushort Quantity { get; init; }
    public required int RequiredBits { get; init; }

    private ModbusDataAddress() { }
    public bool Equals(IDataAddress? other) => Equals(other as ModbusDataAddress);
    // Coil(0, 1)
    public override string ToString() => $"{DataKind}({StartAddress}, {Quantity})";



    public static ModbusDataAddress Coil(byte slaveId, ushort address, ushort quantity)
    {
        return new()
        {
            SlaveId = slaveId,
            DataKind = ModbusDataKind.Coil,
            StartAddress = address,
            Quantity = quantity,
            RequiredBits = quantity
        };
    }
    public static ModbusDataAddress DiscreteInput(byte slaveId, ushort address, ushort quantity)
    {
        return new()
        {
            SlaveId = slaveId,
            DataKind = ModbusDataKind.DiscreteInput,
            StartAddress = address,
            Quantity = quantity,
            RequiredBits = quantity
        };
    }
    public static ModbusDataAddress HoldingRegister(byte slaveId, ushort address, ushort quantity)
    {
        return new()
        {
            SlaveId = slaveId,
            DataKind = ModbusDataKind.HoldingRegister,
            StartAddress = address,
            Quantity = quantity,
            RequiredBits = quantity * 16
        };
    }
    public static ModbusDataAddress InputRegister(byte slaveId, ushort address, ushort quantity)
    {
        return new()
        {
            SlaveId = slaveId,
            DataKind = ModbusDataKind.InputRegister,
            StartAddress = address,
            Quantity = quantity,
            RequiredBits = quantity * 16
        };
    }
}
