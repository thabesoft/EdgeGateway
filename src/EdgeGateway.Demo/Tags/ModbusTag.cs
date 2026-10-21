using EdgeGateway.Bytes.Codecs;
using System.Collections;

namespace EdgeGateway.Tags;


public sealed class ModbusTag<T> : ITag<T>
{
    public required ModbusDataAddress Address { get; init; }
    public required IByteCodec<T> Codec { get; init; }

    IDataAddress ITag<T>.Address => Address;

    internal ModbusTag() { }
}
public sealed record class ModbusDataAddress : IDataAddress
{
    public required byte SlaveId { get; init; }
    public required DataKind DataKind { get; init; }
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
            DataKind = DataKind.Coil,
            StartAddress = address,
            Quantity = quantity
        };
    }
    public static ModbusDataAddress DiscreteInput(byte slaveId, ushort address, ushort quantity)
    {
        return new()
        {
            SlaveId = slaveId,
            DataKind = DataKind.DiscreteInput,
            StartAddress = address,
            Quantity = quantity
        };
    }
    public static ModbusDataAddress HoldingRegister(byte slaveId, ushort address, ushort quantity)
    {
        return new()
        {
            SlaveId = slaveId,
            DataKind = DataKind.HoldingRegister,
            StartAddress = address,
            Quantity = quantity
        };
    }
    public static ModbusDataAddress InputRegister(byte slaveId, ushort address, ushort quantity)
    {
        return new()
        {
            SlaveId = slaveId,
            DataKind = DataKind.InputRegister,
            StartAddress = address,
            Quantity = quantity
        };
    }
}

public enum DataKind
{
    Coil,
    DiscreteInput,
    HoldingRegister,
    InputRegister
}


public static class ModbusTag
{
    public static ModbusTag<bool> Coil(byte slaveId, ushort address)
    {
        return new()
        {
            Address = ModbusDataAddress.Coil(slaveId, address, 1),
            Codec = BoolCodec.BitOffset(0)
        };
    }
    public static ModbusTag<BitArray> Coils(byte slaveId, ushort address, ushort quantity)
    {
        return new()
        {
            Address = ModbusDataAddress.Coil(slaveId, address, quantity),
            Codec = BitCodec.LsbFirst(quantity)
        };
    }

    public static ModbusTag<bool> DiscreteInput(byte slaveId, ushort address)
    {
        return new()
        {
            Address = ModbusDataAddress.DiscreteInput(slaveId, address, 1),
            Codec = BoolCodec.BitOffset(0)
        };
    }
    public static ModbusTag<BitArray> DiscreteInputs(byte slaveId, ushort address, ushort quantity)
    {
        return new()
        {
            Address = ModbusDataAddress.DiscreteInput(slaveId, address, 1),
            Codec = BitCodec.LsbFirst(quantity)
        };
    }


    public static ModbusTag<ushort> HoldingRegister(byte slaveId, ushort address, WordCodec codec)
    {
        return new()
        {
            Address = ModbusDataAddress.HoldingRegister(slaveId, address, 1),
            Codec = codec
        };
    }
    public static ModbusTag<uint> HoldingRegister(byte slaveId, ushort address, DWordCodec codec)
    {
        return new()
        {
            Address = ModbusDataAddress.HoldingRegister(slaveId, address, 2),
            Codec = codec
        };
    }
    public static ModbusTag<ulong> HoldingRegister(byte slaveId, ushort address, QWordCodec codec)
    {
        return new()
        {
            Address = ModbusDataAddress.HoldingRegister(slaveId, address, 4),
            Codec = codec
        };
    }


    public static ModbusTag<ushort> InputRegister(byte slaveId, ushort address, WordCodec codec)
    {
        return new()
        {
            Address = ModbusDataAddress.InputRegister(slaveId, address, 1),
            Codec = codec
        };
    }
    public static ModbusTag<uint> InputRegister(byte slaveId, ushort address, DWordCodec codec)
    {
        return new()
        {
            Address = ModbusDataAddress.InputRegister(slaveId, address, 2),
            Codec = codec
        };
    }
    public static ModbusTag<ulong> InputRegister(byte slaveId, ushort address, QWordCodec codec)
    {
        return new()
        {
            Address = ModbusDataAddress.InputRegister(slaveId, address, 4),
            Codec = codec
        };
    }
}