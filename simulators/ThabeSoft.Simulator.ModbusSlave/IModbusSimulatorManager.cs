using NModbus;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.Simulator.ValueGenerators;

namespace ThabeSoft.Simulator;


public interface IModbusSimulatorManager : ILifecycle
{
    IModbusSimulator AddSlave(byte unitId);
}

public sealed class ModbusSimulatorManager(IModbusFactory modbusFactory, IModbusSlaveNetwork modbusSlaveNetwork) : LifecycleStateMachine, IModbusSimulatorManager
{
    private readonly ConcurrentDictionary<byte, Entry> _entries = [];

    public IModbusSimulator AddSlave(byte unitId)
    {
        if (!_entries.ContainsKey(unitId))
            throw new InvalidOperationException("从站已存在");

        var slave = modbusFactory.CreateSlave(unitId);
        var simulator = new ModbusSimulator(unitId, slave.DataStore);
        var entry = new Entry(simulator, modbusSlaveNetwork, this);

        modbusSlaveNetwork.AddSlave(slave);
        _entries[unitId] = entry;

        return entry;
    }

    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        foreach (var i in _entries.Values)
            await i.StartAsync(cancellationToken);
    }
    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        foreach (var i in _entries.Values)
            await i.StopAsync(cancellationToken);
    }




    internal sealed class Entry(
            IModbusSimulator simulator,
            IModbusSlaveNetwork modbusSlaveNetwork,
            ModbusSimulatorManager manager
        ) : IModbusSimulator
    {
        public async ValueTask DisposeAsync()
        {
            manager._entries.TryRemove(simulator.UnitId, out _);
            modbusSlaveNetwork.RemoveSlave(simulator.UnitId);
            await simulator.DisposeAsync();
        }


        public byte UnitId => simulator.UnitId;
        public LifecycleState State => simulator.State;


        public IDisposable AddValueGenerato<T>(
                ITrigger trigger,
                IValueGenerator<T> generator,
                Action<IModbusSimulatorData, T> update
            ) where T : notnull
            => simulator.AddValueGenerato(trigger, generator, update);



        public void SetCoils(ushort address, params bool[] values) =>
            simulator.SetCoils(address, values);
        public void SetHoldingRegisters(ushort address, params ushort[] values) =>
            simulator.SetHoldingRegisters(address, values);

        public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
            simulator.StartAsync(cancellationToken);
        public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
            simulator.StartAsync(cancellationToken);
    }
}