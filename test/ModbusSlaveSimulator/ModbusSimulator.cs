using Microsoft.Extensions.Logging;
using NModbus;

namespace ThabeSoft.ModbusSimulator;

internal abstract class ModbusSimulator : IModbusSimulator, IAsyncDisposable
{
    private readonly ILogger _logger;
    private readonly CancellationTokenSource _internalTaskCts = new();

    private Task? _slaveNetworkTask;
    private readonly IModbusSlaveNetwork _slaveNetwork;
    private readonly IModbusSlave _slave;

    public byte UnitId => _slave.UnitId;


    protected ModbusSimulator(IModbusFactory modbusFactory, byte unitId, ILogger logger)
    {
        _logger = logger;

        _slaveNetwork = GetModbusSlaveNetwork(modbusFactory);

        _slave = modbusFactory.CreateSlave(unitId);
        _slaveNetwork.AddSlave(_slave);
    }

    protected abstract IModbusSlaveNetwork GetModbusSlaveNetwork(IModbusFactory modbusFactory);


    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        _slaveNetworkTask = Task.Run(async () =>
        {
            _logger.LogInformation("从站已经开始监听");
            await _slaveNetwork.ListenAsync(_internalTaskCts.Token);

        }, _internalTaskCts.Token);
    }
    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        await _internalTaskCts.CancelAsync();

        if (_slaveNetworkTask is not null)
        {
            try { await _slaveNetworkTask; }
            catch { }

            _logger.LogInformation("从站监听已关闭");
        }
    }
    public virtual ValueTask DisposeAsync()
    {
        _internalTaskCts.Dispose();
        return ValueTask.CompletedTask;
    }



    public void SetHoldingRegister(ushort address, ushort value) =>
        _slave.DataStore.HoldingRegisters.WritePoints(address, [value]);
    public ushort GetHoldingRegister(ushort address) =>
        _slave.DataStore.HoldingRegisters.ReadPoints(address, 1)[0];
}
