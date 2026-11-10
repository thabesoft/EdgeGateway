using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Simulation.Networks.Slaves;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Simulation.Networks;


internal abstract class ModbusSimulationNetwork : IModbusSimulationNetwork
{
    private readonly ILifecycleKeyedCollection<byte, ModbusSimulatedSlave> _slaves;
    private readonly IRetryableLifecycleStateMachine _stateMachine;
    private readonly ILifecycleBatchExecutor _batchExecutor;
    private readonly ModbusSimulatedSlaveFactory _slaveSimulatorFactory;


    protected ModbusSimulationNetwork(ModbusSimulatedSlaveFactory slaveSimulatorFactory)
    {
        _slaveSimulatorFactory = slaveSimulatorFactory;

        _batchExecutor = Lifecycle.BatchExecutor();
        _stateMachine = Lifecycle.RetryableStateMachine();
        _slaves = Lifecycle.KeyedCollection<byte, ModbusSimulatedSlave>(_stateMachine);
    }

    public LifecycleState State => _stateMachine.State;


    public virtual async ValueTask<IModbusSimulatedSlave> AddSlaveAsync(byte unitId, CancellationToken cancellationToken)
    {
        return await _stateMachine.ExecuteAsync(ct =>
            _slaves.AddAsync(unitId, () => _slaveSimulatorFactory.Create(unitId), cancellationToken));
    }

    public ValueTask RemoveSalveAsync(byte unitId, CancellationToken cancellationToken)
    {
        return _stateMachine.ExecuteAsync(async _ =>
       {
           await _slaves.RemoveAsync(unitId, cancellationToken);
           OnRemoved(unitId);

       }, cancellationToken);
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);




    protected virtual void OnAdded(ModbusSimulatedSlave slave) { }
    protected virtual void OnRemoved(byte unitId) { }

    protected virtual ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        return _batchExecutor.StartAsync(_slaves.Values, cancellationToken);
    }
    protected virtual ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        return _batchExecutor.StopAsync(_slaves.Values, cancellationToken);
    }
    protected virtual ValueTask OnDisposeAsync()
    {
        return _batchExecutor.DisposeAsync(_slaves.Values);
    }
}