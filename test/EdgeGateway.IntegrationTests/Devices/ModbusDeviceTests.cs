using EdgeGateway.Byte.Codecs;
using EdgeGateway.Modbus.Device;
using EdgeGateway.Modbus.Tag;
using NModbus;
using NModbus.IO;
using System.Collections;
using System.IO.Ports;

namespace EdgeGateway.Device;


[TestClass]
public sealed class ModbusDeviceTests
{
    private SerialPort _slavePort = null!;
    private SerialPort _masterPort = null!;
    private Task? _slaveListenTask;
    private CancellationTokenSource _slaveCts = new();
    private IModbusSlave _modbusSlave = null!;
    private ModbusDevice _device = null!;



    [TestInitialize]
    public void Initialize()
    {
        const string masterPortName = "COM2";
        const string slavePortName = "COM3";
        const int baudRate = 9600;
        const Parity parity = Parity.None;
        const int dataBits = 8;
        const StopBits stopBits = StopBits.One;


        try
        {
            var modbusFactory = new ModbusFactory();

            // 模拟从站
            _slavePort = new SerialPort(slavePortName, baudRate, parity, dataBits, stopBits);
            _slavePort.Open();
            var modbusSlaveNetwork = modbusFactory.CreateRtuSlaveNetwork(new SerialPortStreamResourceWapper(_slavePort));

            // 从站1
            _modbusSlave = modbusFactory.CreateSlave(1);
            modbusSlaveNetwork.AddSlave(_modbusSlave);

            // 从站监听
            _slaveListenTask = Task.Run(async () =>
            {
                try
                {
                    await modbusSlaveNetwork.ListenAsync(_slaveCts.Token);
                }
                catch { }
            }, _slaveCts.Token);



            // 主站
            _masterPort = new SerialPort(masterPortName, baudRate, parity, dataBits, stopBits);
            _masterPort.Open();

            var modbusMaster = modbusFactory.CreateRtuMaster(new SerialPortStreamResourceWapper(_masterPort));
            _device = new ModbusDevice(modbusMaster);
        }
        catch
        {
            _masterPort?.Dispose();
            _slavePort?.Dispose();
            throw;
        }
    }

    [TestCleanup]
    public async Task CleanupAsync()
    {
        _masterPort?.Dispose();
        _slavePort?.Dispose();

        _slaveCts.Cancel();
        if (_slaveListenTask is not null)
        {
            try { await _slaveListenTask; }
            catch { }
        }
        _slaveCts.Dispose();

        await Task.Delay(300, TestContext.CancellationToken);
    }



    [TestMethod]
    [DataRow((ushort)100, true)]
    [DataRow((ushort)100, false)]
    public async Task ReadAsync_Coil(ushort address, bool value)
    {
        _modbusSlave.DataStore.CoilDiscretes.WritePoints(address, [value]);

        var result = await _device.ReadAsync(ModbusTag.Coil(_modbusSlave.UnitId, address), TestContext.CancellationToken);
        Assert.AreEqual(value, result);
    }

    [TestMethod]
    [DataRow((ushort)100, true, false, true)]
    [DataRow((ushort)(ushort.MaxValue - 3), true, true, true)]
    public async Task ReadAsync_Coils(ushort address, bool v1, bool v2, bool v3)
    {
        _modbusSlave.DataStore.CoilDiscretes.WritePoints(address, [v1, v2, v3]);

        var result = await _device.ReadAsync(ModbusTag.Coils(_modbusSlave.UnitId, address, 3), TestContext.CancellationToken);

        Assert.AreSequenceEqual(new BitArray([v1, v2, v3]), result);
    }

    [TestMethod]
    [DataRow((ushort)100, (ushort)12345)]
    public async Task ReadAsync_HoldingRegister_Word(ushort address, ushort value)
    {
        var codec = WordCodec.BigEndian;

        _modbusSlave.DataStore.HoldingRegisters.WritePoints(address, [value]);

        var result = await _device.ReadAsync(ModbusTag.HoldingRegister(_modbusSlave.UnitId, address, codec), TestContext.CancellationToken);

        Assert.AreEqual(value, result);
    }


    public TestContext TestContext { get; set; }
}


public sealed class SerialPortStreamResourceWapper(SerialPort serialPort) : IStreamResource
{
    public int InfiniteTimeout => -1;
    public int ReadTimeout { get => serialPort.ReadTimeout; set => serialPort.ReadTimeout = value; }
    public int WriteTimeout { get => serialPort.WriteTimeout; set => serialPort.WriteTimeout = value; }



    public int Read(byte[] buffer, int offset, int count)
    {
        return serialPort.Read(buffer, offset, count);
    }

    public void Write(byte[] buffer, int offset, int count)
    {
        serialPort.Write(buffer, offset, count);
    }

    public void DiscardInBuffer()
    {
        serialPort.DiscardInBuffer();
    }

    public void Dispose()
    {

    }
}
