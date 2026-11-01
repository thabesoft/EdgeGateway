using Moq;
using NModbus;
using System.Collections;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.Tag;


[TestClass]
public class ModbusTagAccessorTests
{
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task ReadAsync_Coil(bool value)
    {
        var mockMaster = new Mock<IModbusMaster>();
        mockMaster.Setup(x => x.ReadCoilsAsync(It.IsAny<byte>(), It.IsAny<ushort>(), It.IsAny<ushort>()))
            .ReturnsAsync([value]);

        var device = new ModbusTagAccessor(mockMaster.Object);
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

        var device = new ModbusTagAccessor(mockMaster.Object);
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

        var device = new ModbusTagAccessor(mockMaster.Object);
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

        var device = new ModbusTagAccessor(mockMaster.Object);
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

        var device = new ModbusTagAccessor(mockMaster.Object);
        var readResult = await device.ReadAsync(
            ModbusTag.HoldingRegister(0x01, 0x02, QWordCodec.BigEndian),
            TestContext.CancellationToken);

        Assert.AreEqual(value, readResult);

        mockMaster.Verify(x => x.ReadHoldingRegistersAsync(0x01, 0x02, 4), Times.Once);
    }




    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task WriteAsync_Coil(bool value)
    {
        var mockMaster = new Mock<IModbusMaster>();
        mockMaster.Setup(x => x.WriteSingleCoilAsync(It.IsAny<byte>(), It.IsAny<ushort>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        var device = new ModbusTagAccessor(mockMaster.Object);
        await device.WriteAsync(ModbusTag.Coil(0x01, 0x02), value, TestContext.CancellationToken);

        mockMaster.Verify(x => x.WriteSingleCoilAsync(0x01, 0x02, value), Times.Once);
    }

    [TestMethod]
    public async Task WriteAsync_Coils()
    {
        var mockMaster = new Mock<IModbusMaster>();
        mockMaster.Setup(x => x.WriteMultipleCoilsAsync(
                It.IsAny<byte>(), It.IsAny<ushort>(), It.IsAny<bool[]>()))
            .Returns(Task.CompletedTask);

        var bits = new BitArray([true, false, true, false]);   // 4 个线圈
        var device = new ModbusTagAccessor(mockMaster.Object);

        await device.WriteAsync(
            ModbusTag.Coils(0x01, 0x02, quantity: 4),
            bits,
            TestContext.CancellationToken);

        // 验证 master 收到的 bool[] 位模式和 BitArray 一致（LSB First）
        mockMaster.Verify(
            x => x.WriteMultipleCoilsAsync(
                0x01,
                0x02,
                It.Is<bool[]>(b =>
                    b.Length == 4
                    && b[0] == true
                    && b[1] == false
                    && b[2] == true
                    && b[3] == false)),
            Times.Once);
    }

    [TestMethod]
    [DataRow((ushort)0)]
    [DataRow((ushort)0x1234)]
    [DataRow(ushort.MaxValue)]
    public async Task WriteAsync_Word(ushort value)
    {
        var mockMaster = new Mock<IModbusMaster>();
        mockMaster.Setup(x => x.WriteSingleRegisterAsync(
                It.IsAny<byte>(), It.IsAny<ushort>(), It.IsAny<ushort>()))
            .Returns(Task.CompletedTask);

        var device = new ModbusTagAccessor(mockMaster.Object);
        await device.WriteAsync(
            ModbusTag.HoldingRegister(0x01, 0x02, WordCodec.BigEndian),
            value,
            TestContext.CancellationToken);

        mockMaster.Verify(
            x => x.WriteSingleRegisterAsync(0x01, 0x02, value),
            Times.Once);
    }

    [TestMethod]
    [DataRow(10u)]
    [DataRow(0x12345678u)]
    [DataRow(uint.MaxValue)]
    public async Task WriteAsync_DWord(uint value)
    {
        var mockMaster = new Mock<IModbusMaster>();
        mockMaster.Setup(x => x.WriteMultipleRegistersAsync(
                It.IsAny<byte>(), It.IsAny<ushort>(), It.IsAny<ushort[]>()))
            .Returns(Task.CompletedTask);

        var device = new ModbusTagAccessor(mockMaster.Object);
        await device.WriteAsync(
            ModbusTag.HoldingRegister(0x01, 0x02, DWordCodec.BigEndian),
            value,
            TestContext.CancellationToken);

        // 验证 master 收到的 ushort[] 是 [高 16 位, 低 16 位]
        mockMaster.Verify(
            x => x.WriteMultipleRegistersAsync(
                0x01,
                0x02,
                It.Is<ushort[]>(w =>
                    w.Length == 2
                    && w[0] == (ushort)(value >> 16)
                    && w[1] == (ushort)(value & 0xFFFF))),
            Times.Once);
    }

    [TestMethod]
    [DataRow(10UL)]
    [DataRow(0x123456789ABCDEF0UL)]
    [DataRow(ulong.MaxValue)]
    public async Task WriteAsync_QWord(ulong value)
    {
        var mockMaster = new Mock<IModbusMaster>();
        mockMaster.Setup(x => x.WriteMultipleRegistersAsync(
                It.IsAny<byte>(), It.IsAny<ushort>(), It.IsAny<ushort[]>()))
            .Returns(Task.CompletedTask);

        var device = new ModbusTagAccessor(mockMaster.Object);
        await device.WriteAsync(
            ModbusTag.HoldingRegister(0x01, 0x02, QWordCodec.BigEndian),
            value,
            TestContext.CancellationToken);

        // 验证 master 收到的 ushort[] 是 [>>48, >>32, >>16, & 0xFFFF]
        mockMaster.Verify(
            x => x.WriteMultipleRegistersAsync(
                0x01,
                0x02,
                It.Is<ushort[]>(w =>
                    w.Length == 4
                    && w[0] == (ushort)(value >> 48)
                    && w[1] == (ushort)(value >> 32)
                    && w[2] == (ushort)(value >> 16)
                    && w[3] == (ushort)(value & 0xFFFF))),
            Times.Once);
    }




    public TestContext TestContext { get; set; }
}
