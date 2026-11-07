using System.Net;

namespace ThabeSoft.Simulator.Ethernet;


public sealed class ModbusEthernetSimulatorDescriptor : IModbusSimulatorDescriptor
{
    public required byte UnitId { get; init; }
    public required IPAddress Ip { get; init; }
    public required int Port { get; init; }
    public IPEndPoint IPEndPoint => new(Ip, Port);


    private ModbusEthernetSimulatorDescriptor() { }
    public static ModbusEthernetSimulatorDescriptor Create(byte unitId, IPAddress ip, int port)
    {
        if (unitId == 0)
            throw new InvalidOperationException("从站Id不可设置为0");
        ArgumentNullException.ThrowIfNull(ip);
        if (port is < IPEndPoint.MinPort or > IPEndPoint.MaxPort)
            throw new ArgumentOutOfRangeException(nameof(port), port, "端口值在无效范围");

        return new() { UnitId = unitId, Ip = ip, Port = port };
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