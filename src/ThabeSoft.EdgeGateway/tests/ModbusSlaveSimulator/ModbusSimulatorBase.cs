using Microsoft.Extensions.Logging;
using NModbus;

namespace ModbusSlaveSimulator;

internal abstract class ModbusSimulatorBase : IModbusSimulator, IAsyncDisposable
{
    private readonly ILogger _logger;
    private readonly CancellationTokenSource _internalTaskCts = new();
    private readonly IModbusFactory _modbusFactory;

    private Task? _slaveNetworkTask;
    private IModbusSlaveNetwork? _slaveNetwork;
    private readonly IModbusSlave _slave;

    public byte UnitId => _slave.UnitId;


    protected ModbusSimulatorBase(IModbusFactory modbusFactory, byte unitId, ILogger logger)
    {
        _modbusFactory = modbusFactory;
        _logger = logger;

        _slave = modbusFactory.CreateSlave(unitId);
    }

    protected abstract IModbusSlaveNetwork GetModbusSlaveNetwork(IModbusFactory modbusFactory);


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _slaveNetwork = GetModbusSlaveNetwork(_modbusFactory);
        _slaveNetwork.AddSlave(_slave);

        await OnStartAsync(cancellationToken);

        _slaveNetworkTask = Task.Run(async () =>
        {
            _logger.LogInformation("从站已经开始监听");
            await _slaveNetwork.ListenAsync(_internalTaskCts.Token);

        }, _internalTaskCts.Token);
    }
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _internalTaskCts.CancelAsync();
        await OnStopAsync(cancellationToken);

        if (_slaveNetworkTask is not null)
        {
            try { await _slaveNetworkTask; }
            catch { }

            _logger.LogInformation("从站监听已关闭");
        }

        _slaveNetwork?.Dispose();
    }
    public async ValueTask DisposeAsync()
    {
        _internalTaskCts.Dispose();
        await OnDisposeAsync();
    }


    protected virtual ValueTask OnStartAsync(CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    protected virtual ValueTask OnStopAsync(CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;
    protected virtual ValueTask OnDisposeAsync() =>
        ValueTask.CompletedTask;



    public void SetHoldingRegister(ushort address, ushort value) =>
        _slave.DataStore.HoldingRegisters.WritePoints(address, [value]);
    public ushort GetHoldingRegister(ushort address) =>
        _slave.DataStore.HoldingRegisters.ReadPoints(address, 1)[0];
}
