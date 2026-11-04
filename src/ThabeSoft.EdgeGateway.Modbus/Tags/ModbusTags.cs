using System.Collections;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Codecs;

namespace ThabeSoft.EdgeGateway.Tags;


public static class ModbusTags
{
    #region --完整API--

    public static TagDescriptor<bool> Coil(
        TagId name, byte slaveId, ushort address, int bitOffset = 0) => TagDescriptor.Create(
            name, ModbusDataAddress.Coil(slaveId, address, 1), BoolCodec.BitOffset(bitOffset));
    public static TagDescriptor<BitArray> Coils(
        TagId name, byte slaveId, ushort address, ushort quantity) => TagDescriptor.Create(
            name, ModbusDataAddress.Coil(slaveId, address, quantity), BitCodec.LsbFirst(quantity));


    public static TagDescriptor<bool> DiscreteInput(
        TagId name, byte slaveId, ushort address) => TagDescriptor.Create(
            name, ModbusDataAddress.DiscreteInput(slaveId, address, 1), BoolCodec.BitOffset(0));
    public static TagDescriptor<BitArray> DiscreteInputs(
        TagId name, byte slaveId, ushort address, ushort quantity) => TagDescriptor.Create(
            name, ModbusDataAddress.DiscreteInput(slaveId, address, quantity), BitCodec.LsbFirst(quantity));


    public static TagDescriptor<ushort> HoldingRegister(
        TagId name, byte slaveId, ushort address, WordCodec codec) => TagDescriptor.Create(
            name, ModbusDataAddress.HoldingRegister(slaveId, address, 1), codec);
    public static TagDescriptor<uint> HoldingRegister(
        TagId name, byte slaveId, ushort address, DWordCodec codec) => TagDescriptor.Create(
            name, ModbusDataAddress.HoldingRegister(slaveId, address, 2), codec);
    public static TagDescriptor<ulong> HoldingRegister(
        TagId name, byte slaveId, ushort address, QWordCodec codec) => TagDescriptor.Create(
            name, ModbusDataAddress.HoldingRegister(slaveId, address, 4), codec);



    public static TagDescriptor<ushort> InputRegister(
        TagId name, byte slaveId, ushort address, WordCodec codec) => TagDescriptor.Create(
            name, ModbusDataAddress.InputRegister(slaveId, address, 1), codec);
    public static TagDescriptor<uint> InputRegister(
        TagId name, byte slaveId, ushort address, DWordCodec codec) => TagDescriptor.Create(
            name, ModbusDataAddress.InputRegister(slaveId, address, 2), codec);
    public static TagDescriptor<ulong> InputRegister(
        TagId name, byte slaveId, ushort address, QWordCodec codec) => TagDescriptor.Create(
            name, ModbusDataAddress.InputRegister(slaveId, address, 4), codec);

    #endregion

    #region --自动创建Id--

    public static TagDescriptor<bool> Coil(byte slaveId, ushort address) =>
        Coil(TagId.Create(), slaveId, address);
    public static TagDescriptor<BitArray> Coils(byte slaveId, ushort address, ushort quantity) =>
        Coils(TagId.Create(), slaveId, address, quantity);


    public static TagDescriptor<bool> DiscreteInput(byte slaveId, ushort address) =>
        DiscreteInput(TagId.Create(), slaveId, address);
    public static TagDescriptor<BitArray> DiscreteInputs(byte slaveId, ushort address, ushort quantity) =>
        DiscreteInputs(TagId.Create(), slaveId, address, quantity);


    public static TagDescriptor<ushort> HoldingRegister(byte slaveId, ushort address, WordCodec codec) =>
        HoldingRegister(TagId.Create(), slaveId, address, codec);
    public static TagDescriptor<uint> HoldingRegister(byte slaveId, ushort address, DWordCodec codec) =>
        HoldingRegister(TagId.Create(), slaveId, address, codec);
    public static TagDescriptor<ulong> HoldingRegister(byte slaveId, ushort address, QWordCodec codec) =>
        HoldingRegister(TagId.Create(), slaveId, address, codec);


    public static TagDescriptor<ushort> InputRegister(byte slaveId, ushort address, WordCodec codec) =>
        InputRegister(TagId.Create(), slaveId, address, codec);
    public static TagDescriptor<uint> InputRegister(byte slaveId, ushort address, DWordCodec codec) =>
        InputRegister(TagId.Create(), slaveId, address, codec);
    public static TagDescriptor<ulong> InputRegister(byte slaveId, ushort address, QWordCodec codec) =>
        InputRegister(TagId.Create(), slaveId, address, codec);

    #endregion
}