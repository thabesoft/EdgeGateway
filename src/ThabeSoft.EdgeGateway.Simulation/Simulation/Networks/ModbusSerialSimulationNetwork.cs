using NModbus;
using NModbus.Serial;
using System.IO.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Simulation.Networks.Slaves;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Simulation.Networks;


internal sealed class ModbusSerialSimulationNetwork : ModbusSimulationNetwork
{
    private readonly SerialPort _serialPort;
    private IModbusSlaveNetwork _slaveNetWork;
    private Task? _slaveNetworkTask;
    private readonly CancellationTokenSource _slaveNetworkTaskCts = new();


    private ModbusSerialSimulationNetwork(
            ModbusFrameFormat modbusFrameFormat,
            SerialTransportDescriptor serialTransportDescriptor,
            IModbusFactory modbusFactory,
            ModbusSimulatedSlaveFactory modbusSlaveSimulatorFactory
        ) : base(modbusSlaveSimulatorFactory)
    {
        _serialPort = new(
            portName: serialTransportDescriptor.PortName,
            baudRate: serialTransportDescriptor.BaudRate,
            parity: serialTransportDescriptor.Parity.ToSystem(),
            dataBits: serialTransportDescriptor.DataBits,
            stopBits: serialTransportDescriptor.StopBits.ToSystem());

        var serialPortAdapter = new SerialPortAdapter(_serialPort);

        _slaveNetWork = modbusFrameFormat switch
        {
            ModbusFrameFormat.Rtu => modbusFactory.CreateRtuSlaveNetwork(serialPortAdapter),
            ModbusFrameFormat.Ascii => modbusFactory.CreateAsciiSlaveNetwork(serialPortAdapter),
            _ => throw new InvalidOperationException("不支持的帧格式"),
        };
    }
    public static ModbusSerialSimulationNetwork Rtu(
            SerialTransportDescriptor serialTransportDescriptor,
            IModbusFactory modbusFactory,
            ModbusSimulatedSlaveFactory modbusSlaveSimulatorFactory)
    {
        return new(ModbusFrameFormat.Rtu, serialTransportDescriptor, modbusFactory, modbusSlaveSimulatorFactory);
    }
    public static ModbusSerialSimulationNetwork Ascii(
            SerialTransportDescriptor serialTransportDescriptor,
            IModbusFactory modbusFactory,
            ModbusSimulatedSlaveFactory modbusSlaveSimulatorFactory)
    {
        return new(ModbusFrameFormat.Ascii, serialTransportDescriptor, modbusFactory, modbusSlaveSimulatorFactory);
    }


    public byte UnitId { get; }



    protected override void OnAdded(ModbusSimulatedSlave simulated)
    {
        _slaveNetWork.AddSlave(simulated.Slave);
    }
    protected override void OnRemoved(ModbusSimulatedSlave slave)
    {
        _slaveNetWork.RemoveSlave(slave.UnitId);
    }



    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        _serialPort.Open();
        _slaveNetworkTask = Task.Run(async () => await _slaveNetWork.ListenAsync(), _slaveNetworkTaskCts.Token);

        await base.OnStartAsync(cancellationToken);
    }
    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        await base.OnStopAsync(cancellationToken);
        _serialPort.Close();
    }
    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        _serialPort.Dispose();

        await _slaveNetworkTaskCts.CancelAsync();
        if (_slaveNetworkTask is not null)
        {
            try { await _slaveNetworkTask; }
            catch { }
        }

        _slaveNetWork.Dispose();
    }
}