using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Simulation.Networks.Slaves;

namespace ThabeSoft.EdgeGateway.Simulation.Networks;


internal abstract class ModbusSimulationNetwork(
        ModbusSimulatedSlaveFactory slaveSimulatorFactory
    ) : LifecycleStateMachine, IModbusSimulationNetwork
{
    private readonly ConcurrentDictionary<byte, ModbusSimulatedSlave> _slaves = [];

    public virtual IModbusSimulatedSlave AddSlave(byte unitId)
    {
        if (_slaves.ContainsKey(unitId))
            throw new InvalidOperationException("从站已存在");

        var simulator = slaveSimulatorFactory.Create(unitId);
        _slaves[unitId] = simulator;

        return simulator;
    }
    public virtual async Task RemoveSlave(byte unitId)
    {
        if (!_slaves.TryRemove(unitId, out var item))
            return;

        await item.DisposeAsync();
    }


    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        foreach (var i in _slaves.Values)
            await i.StartAsync(cancellationToken);
    }
    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        foreach (var i in _slaves.Values)
            await i.StopAsync(cancellationToken);
    }
}