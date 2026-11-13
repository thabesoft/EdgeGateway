using Microsoft.Extensions.Logging;
using NModbus;
using NModbus.Serial;
using System.IO.Ports;
using ThabeSoft.EdgeGateway.Devices.Modbus;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport;

using SystemStopBit = System.IO.Ports.StopBits;
using SystemParity = System.IO.Ports.Parity;
using ThabeSoft.EdgeGateway.Devices;

namespace ThabeSoft.EdgeGateway.Ports.Modbus;


internal sealed class ModbusSerialSimulationPort : SimulationPortBase
{
    private readonly SerialPort _serialPort;
    private IModbusSlaveNetwork _slaveNetWork;
    private Task? _slaveNetworkTask;
    private readonly CancellationTokenSource _slaveNetworkTaskCts = new();


    private ModbusSerialSimulationPort(
            ModbusFrameFormat modbusFrameFormat,
            SerialTransportDescriptor serialTransportDescriptor,
            IModbusFactory modbusFactory,
            SimulationDeviceFactorySelector deviceFactorySelector,
            ILogger<ModbusSerialSimulationPort>? logger = null
        ) : base(serialTransportDescriptor.Endpoint, deviceFactorySelector, logger)
    {
        _serialPort = new(
            portName: serialTransportDescriptor.PortName,
            baudRate: serialTransportDescriptor.BaudRate,
            parity: Parse(serialTransportDescriptor.Parity),
            dataBits: serialTransportDescriptor.DataBits,
            stopBits: Parse(serialTransportDescriptor.StopBits));

        var serialPortAdapter = new SerialPortAdapter(_serialPort);

        _slaveNetWork = modbusFrameFormat switch
        {
            ModbusFrameFormat.Rtu => modbusFactory.CreateRtuSlaveNetwork(serialPortAdapter),
            ModbusFrameFormat.Ascii => modbusFactory.CreateAsciiSlaveNetwork(serialPortAdapter),
            _ => throw new InvalidOperationException("不支持的帧格式"),
        };
    }
    public static ModbusSerialSimulationPort Rtu(
            SerialTransportDescriptor serialTransportDescriptor,
            IModbusFactory modbusFactory,
            SimulationDeviceFactorySelector deviceFactorySelector,
            ILogger<ModbusSerialSimulationPort>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(serialTransportDescriptor);
        ArgumentNullException.ThrowIfNull(modbusFactory);
        ArgumentNullException.ThrowIfNull(deviceFactorySelector);

        return new(ModbusFrameFormat.Rtu, serialTransportDescriptor, modbusFactory, deviceFactorySelector, logger);
    }
    public static ModbusSerialSimulationPort Ascii(
            SerialTransportDescriptor serialTransportDescriptor,
            IModbusFactory modbusFactory,
            SimulationDeviceFactorySelector deviceFactorySelector,
            ILogger<ModbusSerialSimulationPort>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(serialTransportDescriptor);
        ArgumentNullException.ThrowIfNull(modbusFactory);
        ArgumentNullException.ThrowIfNull(deviceFactorySelector);

        return new(ModbusFrameFormat.Ascii, serialTransportDescriptor, modbusFactory, deviceFactorySelector, logger);
    }


    public byte UnitId { get; }



    protected override void OnAdded(ISimulationDevice device)
    {
        if (device is not ModbusSimulationDevice modbus)
            return;

        _slaveNetWork.AddSlave(modbus.Slave);
    }
    protected override void OnRemoved(ISimulationDeviceId deviceId)
    {
        if (deviceId is not ModbusSimulationDeviceId modbus)
            return;

        _slaveNetWork.RemoveSlave(modbus.Value);
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



    public static SystemParity Parse(SerialParity parity) => parity switch
    {
        SerialParity.None => SystemParity.None,
        SerialParity.Even => SystemParity.Even,
        SerialParity.Odd => SystemParity.Odd,
        SerialParity.Mark => SystemParity.Mark,
        SerialParity.Space => SystemParity.Space,
        _ => throw new ArgumentOutOfRangeException(nameof(parity))
    };

    public static SystemStopBit Parse(SerialStopBits stopBits) => stopBits switch
    {
        SerialStopBits.One => SystemStopBit.One,
        SerialStopBits.Two => SystemStopBit.Two,
        SerialStopBits.OnePointFive => SystemStopBit.OnePointFive,
        _ => throw new ArgumentOutOfRangeException(nameof(stopBits))
    };
}