using ThabeSoft.Codecs;
using ThabeSoft.Protocols.Accessor;
using ThabeSoft.Protocols.Modbus;

namespace ThabeSoft.UnitTests.Protocols;


/// <summary>
/// 校验 Modbus 标签描述符：地址位数必须与 Codec 位数一致，
/// 且寄存器数量由 Codec 位数推导（16bit→1、32bit→2、64bit→4）。
/// </summary>
[TestClass]
public class ModbusDataAccessTests
{
    [TestMethod]
    public void HoldingRegister_QuantityFollowsCodecBits()
    {
        Assert.AreEqual((ushort)1, AddressOf(Modbus.HoldingRegister(1, 0, Codec.UInt16BigEndian)).Quantity);
        Assert.AreEqual((ushort)2, AddressOf(Modbus.HoldingRegister(1, 0, Codec.Int32Abcd)).Quantity);
        Assert.AreEqual((ushort)2, AddressOf(Modbus.HoldingRegister(1, 0, Codec.FloatAbcd)).Quantity);
        Assert.AreEqual((ushort)4, AddressOf(Modbus.HoldingRegister(1, 0, Codec.Int64BigEndian)).Quantity);
        Assert.AreEqual((ushort)4, AddressOf(Modbus.HoldingRegister(1, 0, Codec.DoubleBigEndian)).Quantity);
    }

    [TestMethod]
    public void InputRegister_QuantityFollowsCodecBits()
    {
        Assert.AreEqual((ushort)1, AddressOf(Modbus.InputRegister(1, 0, Codec.UInt16LittleEndian)).Quantity);
        Assert.AreEqual((ushort)2, AddressOf(Modbus.InputRegister(1, 0, Codec.UInt32Abcd)).Quantity);
        Assert.AreEqual((ushort)4, AddressOf(Modbus.InputRegister(1, 0, Codec.UInt64LittleEndian)).Quantity);
    }

    [TestMethod]
    public void Codec_BitsMustBeMultipleOf16()
    {
        Assert.Throws<InvalidOperationException>(() => Modbus.HoldingRegister(1, 0, Codec.Byte));
    }


    private static ModbusProtocolAddress AddressOf<T>(DataAccessDescriptor<T> descriptor) where T : notnull
        => Assert.IsInstanceOfType<ModbusProtocolAddress>(descriptor.Address);
}
