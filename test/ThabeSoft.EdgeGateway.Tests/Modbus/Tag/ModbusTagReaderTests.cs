using EdgeGateway.Byte.Codecs;
using Moq;
using NModbus;
using ThabeSoft.EdgeGateway.Modbus;

namespace EdgeGateway.Modbus.Tag;


[TestClass]
public class ModbusTagReaderTests
{
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task ReadAsync_Coil(bool value)
    {
        var mockMaster = new Mock<IModbusMaster>();
        mockMaster.Setup(x => x.ReadCoilsAsync(It.IsAny<byte>(), It.IsAny<ushort>(), It.IsAny<ushort>()))
            .ReturnsAsync([value]);

        var device = new ModbusTagReader(mockMaster.Object);
        var readResult = await device.ReadAsync(ModbusTag.Coil(0x01, 0x02), TestContext.CancellationToken);

        Assert.AreEqual(value, readResult);
    }

    [TestMethod]
    [DataRow(true, false, true, false)]
    [DataRow(true, true, true, false)]
    [DataRow(true, true, true, true)]
    public async Task ReadAsync_Coils(bool a, bool b, bool c, bool d)
    {
        var mockMaster = new Mock<IModbusMaster>();
        mockMaster.Setup(x => x.ReadCoilsAsync(It.IsAny<byte>(), It.IsAny<ushort>(), It.IsAny<ushort>()))
            .ReturnsAsync([a, b, c, d]);

        var device = new ModbusTagReader(mockMaster.Object);
        var readResult = await device.ReadAsync(ModbusTag.Coils(0x01, 0x02, 4), TestContext.CancellationToken);

        bool[] expected = [a, b, c, d];
        Assert.AreSequenceEqual(expected, readResult);
    }

    [TestMethod]
    [DataRow(ushort.MinValue)]
    [DataRow(ushort.MaxValue)]
    [DataRow((ushort)1)]
    [DataRow((ushort)0x1234)]
    public async Task ReadAsync_HoldingRegister_Word(ushort value)
    {
        var mockMaster = new Mock<IModbusMaster>();
        mockMaster.Setup(x => x.ReadHoldingRegistersAsync(It.IsAny<byte>(), It.IsAny<ushort>(), It.IsAny<ushort>()))
            .ReturnsAsync([value]);

        var device = new ModbusTagReader(mockMaster.Object);
        var readResult = await device.ReadAsync(ModbusTag.HoldingRegister(0x01, 0x02, WordCodec.BigEndian), TestContext.CancellationToken);

        Assert.AreEqual(value, readResult);
        mockMaster.Verify(x => x.ReadHoldingRegistersAsync(0x01, 0x02, 1), Times.Once);
    }

    [TestMethod]
    [DataRow(uint.MinValue)]
    [DataRow(uint.MaxValue)]
    [DataRow(0x12345678u)]
    [DataRow(0x00000001u)]
    [DataRow(0x00010000u)]
    public async Task ReadAsync_HoldingRegister_DWord(uint value)
    {
        var mockMaster = new Mock<IModbusMaster>();
        mockMaster.Setup(x => x.ReadHoldingRegistersAsync(It.IsAny<byte>(), It.IsAny<ushort>(), It.IsAny<ushort>()))
            .ReturnsAsync([(ushort)(value >> 16), (ushort)(value & 0xFFFF)]);

        var device = new ModbusTagReader(mockMaster.Object);
        var readResult = await device.ReadAsync(ModbusTag.HoldingRegister(0x01, 0x02, DWordCodec.BigEndian), TestContext.CancellationToken);

        Assert.AreEqual(value, readResult);
        mockMaster.Verify(x => x.ReadHoldingRegistersAsync(0x01, 0x02, 2), Times.Once);
    }


    [TestMethod]
    [DataRow(10UL)]
    [DataRow(ulong.MinValue)]
    [DataRow(ulong.MaxValue)]
    [DataRow(0x123456789ABCDEF0UL)]
    [DataRow(0x0000000000000001UL)]
    [DataRow(0x0001000000000000UL)]
    public async Task ReadAsync_HoldingRegister_QWord(ulong value)
    {
        var mockMaster = new Mock<IModbusMaster>();
        mockMaster.Setup(x => x.ReadHoldingRegistersAsync(It.IsAny<byte>(), It.IsAny<ushort>(), It.IsAny<ushort>()))
            .ReturnsAsync([(ushort)(value >> 48), (ushort)(value >> 32), (ushort)(value >> 16), (ushort)(value & 0xFFFF)]);

        var device = new ModbusTagReader(mockMaster.Object);
        var readResult = await device.ReadAsync(
            ModbusTag.HoldingRegister(0x01, 0x02, QWordCodec.BigEndian),
            TestContext.CancellationToken);

        Assert.AreEqual(value, readResult);

        mockMaster.Verify(x => x.ReadHoldingRegistersAsync(0x01, 0x02, 4), Times.Once);
    }

    public TestContext TestContext { get; set; }
}
