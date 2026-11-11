using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NModbus;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Simulation.ValueGenerators;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Simulation.Networks.Slaves;

public interface IModbusSimulationSlave : ILifecycle, IAsyncDisposable
{
    internal IModbusSlave Slave { get; }
    IDisposable Value<T>(TagDescriptor<T> tagDescriptor, ITriggerDescriptor triggerDescriptor, IValueGenerator<T> generator) where T : notnull;
}

internal sealed class ModbusSimulationSlave(
        byte unitId,
        IModbusFactory modbusFactory,
        ITriggerFactorySelector triggerFactorySelector,
        ILogger<ModbusSimulationSlave>? logger = null
    ) : IModbusSimulationSlave
{
    private readonly IStrictLifecycleStateMachine _stateMachine = Lifecycle.StrictStateMachine();
    private readonly TaskManager _taskManager = new();
    private readonly ConcurrentDictionary<Guid, ITrigger> _triggers = [];
    private ILogger<ModbusSimulationSlave> _logger = logger ?? NullLogger<ModbusSimulationSlave>.Instance;


    public byte UnitId => unitId;
    public IModbusSlave Slave { get; } = modbusFactory.CreateSlave(unitId);
    public LifecycleState State => _stateMachine.State;




    public IDisposable Value<T>(TagDescriptor<T> tagDescriptor, ITriggerDescriptor triggerDescriptor, IValueGenerator<T> generator)
        where T : notnull
    {
        if (tagDescriptor.Address is not ModbusDataAddress dataAddrss)
            throw new InvalidOperationException("不支持的标签");

        if (tagDescriptor.Codec is not IRequiredBits requiredBits)
            throw new InvalidOperationException("不支持的标签");

        var trigger = triggerFactorySelector.Create(triggerDescriptor);
        _triggers[Guid.NewGuid()] = trigger;


        return _taskManager.AddTask(async ct =>
        {
            _logger.LogTrace("添加生成值");

            while (!ct.IsCancellationRequested)
            {
                if (!await trigger.WaitForNextTickAsync(ct))
                    return;

                var value = generator.Next();

                if (dataAddrss.Kind == ModbusDataKind.HoldingRegister)
                {
                    var words = tagDescriptor.Codec.EncodeWords(value);
                    SetHoldingRegisters(dataAddrss.StartAddress, words);
                }
            }
        });
    }


    public void SetCoils(ushort address, params bool[] values)
    {
        Slave.DataStore.CoilInputs.WritePoints(address, values);
        _logger.LogTrace("Slave[{Id}] 设置线圈值: ({Address},{values})", UnitId, address, string.Join(string.Empty, values.Select(x => x ? '1' : '0')));
    }
    public void SetHoldingRegisters(ushort address, params ushort[] values)
    {
        Slave.DataStore.HoldingRegisters.WritePoints(address, values);
        _logger.LogTrace("Slave[{Id}] 设置保持寄存器值: ({Address},{values})", UnitId, address, string.Join(" ", values));
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(_taskManager.StartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(_taskManager.StopAsync, cancellationToken);
    public ValueTask DisposeAsync() => _stateMachine.DisposeAsync(async () =>
    {
        foreach (var i in _triggers.Values)
            await i.DisposeAsync();

        await _taskManager.DisposeAsync();
    });
}
