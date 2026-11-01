using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NModbus;
using NModbus.Serial;
using System.IO.Ports;

namespace ThabeSoft.ModbusSimulator;

public interface IModbusSerialSimulator : IModbusSimulator
{
    SerialPortOptions SerialPortOptions { get; }
}

internal sealed class ModbusRtuSimulator(
        IModbusFactory modbusFactory,
        IOptions<ModbusSerialSimulatorOptions> options,
        ILogger<ModbusRtuSimulator> logger
    ) : ModbusSimulator(modbusFactory, options.Value.UnitId, logger), IModbusSerialSimulator
{
    private SerialPort? _serialPort;
    public SerialPortOptions SerialPortOptions => options.Value.SerialPortOptions;


    protected override IModbusSlaveNetwork GetModbusSlaveNetwork(IModbusFactory modbusFactory)
    {
        _serialPort = new(
            portName: options.Value.SerialPortOptions.PortName,
            baudRate: options.Value.SerialPortOptions.BaudRate,
            parity: options.Value.SerialPortOptions.Parity,
            dataBits: options.Value.SerialPortOptions.DataBits,
            stopBits: options.Value.SerialPortOptions.StopBits);

        return modbusFactory.CreateRtuSlaveNetwork(new SerialPortAdapter(_serialPort));
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _serialPort?.Open();
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _serialPort?.Close();
        await base.StopAsync(cancellationToken);
    }
    public override ValueTask DisposeAsync()
    {
        _serialPort?.Dispose();
        return base.DisposeAsync();
    }
}



///// <summary>
///// 此任务会开启串口, 并创建从站
///// </summary>
//internal sealed class ModbusRtuSlave : IHostedService, IDisposable
//{
//    private readonly IOptions<ModbusRtuSlaveOptions> _options;
//    private readonly ILogger<ModbusRtuSlave> _logger;
//    private readonly CancellationTokenSource _internalTaskCts = new();

//    private readonly SerialPort _serialPort;
//    private Task? _slaveNetworkTask;
//    private readonly IModbusSlaveNetwork _slaveNetwork;
//    private readonly IModbusSlave _slave1;

//    private Task? _tickTask;
//    private PeriodicTimer? _timer;


//    public ModbusRtuSlave(IModbusFactory modbusFactory, IOptions<ModbusRtuSlaveOptions> options, ILogger<ModbusRtuSlave> logger)
//    {
//        _options = options;
//        _logger = logger;

//        _serialPort = new(
//            portName: options.Value.PortName,
//            baudRate: options.Value.BaudRate,
//            parity: options.Value.Parity,
//            dataBits: options.Value.DataBits,
//            stopBits: options.Value.StopBits);

//        _slaveNetwork = modbusFactory.CreateRtuSlaveNetwork(new SerialPortAdapter(_serialPort));

//        _slave1 = modbusFactory.CreateSlave(1);
//        _slaveNetwork.AddSlave(_slave1);
//    }


//    public async Task StartAsync(CancellationToken _)
//    {
//        _serialPort.Open();
//        _logger.LogInformation("串口 {name} 已经打开", _options.Value.PortName);


//        _slaveNetworkTask = Task.Run(async () =>
//        {
//            _logger.LogInformation("从站已经开始监听");
//            await _slaveNetwork.ListenAsync(_internalTaskCts.Token);

//        }, _internalTaskCts.Token);

//        _tickTask = Task.Run(async () =>
//        {
//            _logger.LogInformation("已经更新从站数据");
//            _timer = new(_options.Value.TickInterval);

//            while (!_internalTaskCts.IsCancellationRequested)
//            {
//                if (!await _timer.WaitForNextTickAsync(_internalTaskCts.Token))
//                    return;

//                await OnTickAsync(_internalTaskCts.Token);
//            }
//        }, _internalTaskCts.Token);
//    }

//    public async Task StopAsync(CancellationToken cancellationToken)
//    {
//        await _internalTaskCts.CancelAsync();
//        _serialPort.Close();

//        if (_slaveNetworkTask is not null)
//        {
//            try { await _slaveNetworkTask; }
//            catch { }

//            _logger.LogInformation("从站监听已关闭");
//        }
//        if (_tickTask is not null)
//        {
//            try { await _tickTask; }
//            catch { }

//            _logger.LogInformation("从站数据更新已关闭");
//        }
//    }
//    public void Dispose()
//    {
//        _internalTaskCts.Dispose();
//        _serialPort.Dispose();
//    }


//    private async Task OnTickAsync(CancellationToken cancellationToken)
//    {
//        // 温度 [Id:1] [Addr:0] [Len:1] [Range:20~30]
//        _slave1.DataStore.HoldingRegisters.WritePoints(0, [Random.Shared.NextInteger<ushort>(10, 30)]);
//    }
//}