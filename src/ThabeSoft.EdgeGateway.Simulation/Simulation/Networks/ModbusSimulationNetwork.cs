using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Simulation.Networks.Slaves;

namespace ThabeSoft.EdgeGateway.Simulation.Networks;


internal abstract class ModbusSimulationNetwork(
        ModbusSimulatedSlaveFactory slaveSimulatorFactory
    ) : IModbusSimulationNetwork
{
    private readonly CollectionLifecycleStateMachine _stateMachine = Lifecycle.CollectionStateMachine();
    private readonly ConcurrentDictionary<byte, ModbusSimulatedSlave> _slaves = [];

    public LifecycleState State => _stateMachine.State;


    public virtual IModbusSimulatedSlave AddSlave(byte unitId)
    {
        return _stateMachine.Execute((UnitId: unitId, Slaves: _slaves, SimulatorFactory: slaveSimulatorFactory, This: this), static state =>
        {
            if (state.Slaves.ContainsKey(state.UnitId))
                throw new InvalidOperationException("从站已存在");

            var simulator = state.SimulatorFactory.Create(state.UnitId);
            state.Slaves[state.UnitId] = simulator;

            state.This.OnAdded(simulator);
            return simulator;
        });
    }

    public void RemoveSalve(byte unitId)
    {
        _stateMachine.Execute((UnitId: unitId, Slaves: _slaves, SimulatorFactory: slaveSimulatorFactory, This: this), static async state =>
       {
           if (!state.Slaves.TryRemove(state.UnitId, out var slave))
               return;

           await slave.DisposeAsync();
           state.This.OnRemoved(slave);
       });
    }



    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAllAsync([Lifecycle.Starter(OnStartAsync), .. _slaves.Values], cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAllAsync([.. _slaves.Values, Lifecycle.Stoper(OnStopAsync)], cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAllAsync([.. _slaves.Values, Lifecycle.Disposer(OnDisposeAsync)]);




    protected virtual void OnAdded(ModbusSimulatedSlave slave) { }
    protected virtual void OnRemoved(ModbusSimulatedSlave slave) { }
    protected virtual ValueTask OnStartAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
    protected virtual ValueTask OnStopAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
    protected virtual ValueTask OnDisposeAsync() => ValueTask.CompletedTask;
}