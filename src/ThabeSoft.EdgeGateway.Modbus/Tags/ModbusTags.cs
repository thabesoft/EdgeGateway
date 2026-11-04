using System.Collections;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Codecs;

namespace ThabeSoft.EdgeGateway.Tags;


public static class ModbusTags
{
    #region --完整API--

    public static TagDescriptor<bool> Coil(
        TagName name, byte slaveId, ushort address, int bitOffset = 0) => TagDescriptor.Create(
            name, ModbusDataAddress.Coil(slaveId, address, 1), BoolCodec.BitOffset(bitOffset));
    public static TagDescriptor<BitArray> Coils(
        TagName name, byte slaveId, ushort address, ushort quantity) => TagDescriptor.Create(
            name, ModbusDataAddress.Coil(slaveId, address, quantity), BitCodec.LsbFirst(quantity));


    public static TagDescriptor<bool> DiscreteInput(
        TagName name, byte slaveId, ushort address) => TagDescriptor.Create(
            name, ModbusDataAddress.DiscreteInput(slaveId, address, 1), BoolCodec.BitOffset(0));
    public static TagDescriptor<BitArray> DiscreteInputs(
        TagName name, byte slaveId, ushort address, ushort quantity) => TagDescriptor.Create(
            name, ModbusDataAddress.DiscreteInput(slaveId, address, quantity), BitCodec.LsbFirst(quantity));


    public static TagDescriptor<ushort> HoldingRegister(
        TagName name, byte slaveId, ushort address, WordCodec codec) => TagDescriptor.Create(
            name, ModbusDataAddress.HoldingRegister(slaveId, address, 1), codec);
    public static TagDescriptor<uint> HoldingRegister(
        TagName name, byte slaveId, ushort address, DWordCodec codec) => TagDescriptor.Create(
            name, ModbusDataAddress.HoldingRegister(slaveId, address, 2), codec);
    public static TagDescriptor<ulong> HoldingRegister(
        TagName name, byte slaveId, ushort address, QWordCodec codec) => TagDescriptor.Create(
            name, ModbusDataAddress.HoldingRegister(slaveId, address, 4), codec);



    public static TagDescriptor<ushort> InputRegister(
        TagName name, byte slaveId, ushort address, WordCodec codec) => TagDescriptor.Create(
            name, ModbusDataAddress.InputRegister(slaveId, address, 1), codec);
    public static TagDescriptor<uint> InputRegister(
        TagName name, byte slaveId, ushort address, DWordCodec codec) => TagDescriptor.Create(
            name, ModbusDataAddress.InputRegister(slaveId, address, 2), codec);
    public static TagDescriptor<ulong> InputRegister(
        TagName name, byte slaveId, ushort address, QWordCodec codec) => TagDescriptor.Create(
            name, ModbusDataAddress.InputRegister(slaveId, address, 4), codec);

    #endregion

    #region --字符串标签名称重载--

    public static TagDescriptor<bool> Coil(
        string name, byte slaveId, ushort address) => Coil(TagName.From(name), slaveId, address);
    public static TagDescriptor<BitArray> Coils(
        string name, byte slaveId, ushort address, ushort quantity) => Coils(TagName.From(name), slaveId, address, quantity);


    public static TagDescriptor<bool> DiscreteInput(
        string name, byte slaveId, ushort address) => DiscreteInput(TagName.From(name), slaveId, address);
    public static TagDescriptor<BitArray> DiscreteInputs(
        string name, byte slaveId, ushort address, ushort quantity) => DiscreteInputs(TagName.From(name), slaveId, address, quantity);


    public static TagDescriptor<ushort> HoldingRegister(
        string name, byte slaveId, ushort address, WordCodec codec) => HoldingRegister(TagName.From(name), slaveId, address, codec);
    public static TagDescriptor<uint> HoldingRegister(
        string name, byte slaveId, ushort address, DWordCodec codec) => HoldingRegister(TagName.From(name), slaveId, address, codec);
    public static TagDescriptor<ulong> HoldingRegister(
        string name, byte slaveId, ushort address, QWordCodec codec) => HoldingRegister(TagName.From(name), slaveId, address, codec);


    public static TagDescriptor<ushort> InputRegister(
        string name, byte slaveId, ushort address, WordCodec codec) => InputRegister(TagName.From(name), slaveId, address, codec);
    public static TagDescriptor<uint> InputRegister(
        string name, byte slaveId, ushort address, DWordCodec codec) => InputRegister(TagName.From(name), slaveId, address, codec);
    public static TagDescriptor<ulong> InputRegister(
        string name, byte slaveId, ushort address, QWordCodec codec) => InputRegister(TagName.From(name), slaveId, address, codec);

    #endregion


    public static TagDescriptor<ushort> HoldingRegisterWord(
        string name, byte slaveId, ushort address) => HoldingRegister(name, slaveId, address, WordCodec.BigEndian);
    public static TagDescriptor<uint> HoldingRegisterDWord(
        string name, byte slaveId, ushort address) => HoldingRegister(TagName.From(name), slaveId, address, DWordCodec.BigEndian);
    public static TagDescriptor<ulong> HoldingRegisterQWord(
        string name, byte slaveId, ushort address) => HoldingRegister(TagName.From(name), slaveId, address, QWordCodec.BigEndian);


    public static TagDescriptor<ushort> InputRegisterWord(
        string name, byte slaveId, ushort address) => InputRegister(TagName.From(name), slaveId, address, WordCodec.BigEndian);
    public static TagDescriptor<uint> InputRegisterDWord(
        string name, byte slaveId, ushort address) => InputRegister(TagName.From(name), slaveId, address, DWordCodec.BigEndian);
    public static TagDescriptor<ulong> InputRegisterQWord(
        string name, byte slaveId, ushort address) => InputRegister(TagName.From(name), slaveId, address, QWordCodec.BigEndian);
}