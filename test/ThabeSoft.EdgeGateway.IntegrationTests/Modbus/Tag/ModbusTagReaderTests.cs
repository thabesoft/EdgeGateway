using EdgeGateway.Byte.Codecs;
using NModbus;
using System.Collections;
using ThabeSoft.EdgeGateway.Modbus;

namespace EdgeGateway.Modbus.Device;


[TestClass]
public sealed class ModbusTagReaderTests
{
    private ScopeSerialPortModbusSlaveNetwork _scopeSlaveNetwork = null!;
    private IModbusSlave _slave1 = null!;

    private ScopeSerialPortModbusTagReader _scopeTagProtocol = null!;



    [TestInitialize]
    public async Task Initialize()
    {
        _scopeSlaveNetwork = new(new("COM3"));
        _slave1 = _scopeSlaveNetwork.AddSlave(1);
        await _scopeSlaveNetwork.OpenAsync();


        _scopeTagProtocol = new(new("COM2"));
        await _scopeTagProtocol.OpenAsync();
    }

    [TestCleanup]
    public async Task CleanupAsync()
    {
        if (_scopeSlaveNetwork is not null)
            await _scopeSlaveNetwork.DisposeAsync();

        if (_scopeTagProtocol is not null)
            await _scopeTagProtocol.DisposeAsync();
    }



    [TestMethod]
    [DataRow((ushort)100, true)]
    [DataRow((ushort)100, false)]
    public async Task ReadAsync_Coil(ushort address, bool value)
    {
        _slave1.DataStore.CoilDiscretes.WritePoints(address, [value]);

        var result = await _scopeTagProtocol.TagReader.ReadAsync(ModbusTag.Coil(_slave1.UnitId, address), TestContext.CancellationToken);
        Assert.AreEqual(value, result);
    }

    [TestMethod]
    [DataRow((ushort)100, true, false, true)]
    [DataRow((ushort)(ushort.MaxValue - 3), true, true, true)]
    public async Task ReadAsync_Coils(ushort address, bool v1, bool v2, bool v3)
    {
        _slave1.DataStore.CoilDiscretes.WritePoints(address, [v1, v2, v3]);

        var result = await _scopeTagProtocol.TagReader.ReadAsync(ModbusTag.Coils(_slave1.UnitId, address, 3), TestContext.CancellationToken);

        Assert.AreSequenceEqual(new BitArray([v1, v2, v3]), result);
    }

    [TestMethod]
    [DataRow((ushort)100, (ushort)12345)]
    public async Task ReadAsync_HoldingRegister_Word(ushort address, ushort value)
    {
        var codec = WordCodec.BigEndian;

        _slave1.DataStore.HoldingRegisters.WritePoints(address, [value]);

        var result = await _scopeTagProtocol.TagReader.ReadAsync(ModbusTag.HoldingRegister(_slave1.UnitId, address, codec), TestContext.CancellationToken);

        Assert.AreEqual(value, result);
    }


    public TestContext TestContext { get; set; }
}