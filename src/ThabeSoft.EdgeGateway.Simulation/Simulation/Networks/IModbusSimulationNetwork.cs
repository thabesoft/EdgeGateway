using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Simulation.Networks.Slaves;

namespace ThabeSoft.EdgeGateway.Simulation.Networks;

public interface IModbusSimulationNetwork : ILifecycle, IAsyncDisposable
{
    ValueTask<IModbusSimulationSlave> AddSlaveAsync(byte unitId, CancellationToken cancellationToken = default);
    ValueTask RemoveSalveAsync(byte unitId, CancellationToken cancellationToken = default);
}

internal abstract class ModbusSimulationNetwork : IModbusSimulationNetwork
{
    private readonly ILifecycleKeyedCollection<byte, ModbusSimulationSlave> _slaves;
    private readonly IRetryableLifecycleStateMachine _stateMachine;
    private readonly ILifecycleBatchExecutor _batchExecutor;
    private readonly ModbusSimulatedSlaveFactory _slaveSimulatorFactory;
    private readonly ILogger _logger;

    protected ModbusSimulationNetwork(ModbusSimulatedSlaveFactory slaveSimulatorFactory, ILogger? logger)
    {
        _slaveSimulatorFactory = slaveSimulatorFactory;
        _logger = logger ?? NullLogger.Instance;


        _batchExecutor = Lifecycle.BatchExecutor(logger);
        _stateMachine = Lifecycle.RetryableStateMachine(logger);
        _slaves = Lifecycle.KeyedCollection<byte, ModbusSimulationSlave>(_stateMachine, logger);
    }

    public LifecycleState State => _stateMachine.State;


    public virtual async ValueTask<IModbusSimulationSlave> AddSlaveAsync(byte unitId, CancellationToken cancellationToken)
    {
        return await _stateMachine.ExecuteAsync(async ct =>
        {
            var slave = await _slaves.AddAsync(unitId, () => _slaveSimulatorFactory.Create(unitId), ct);
            _logger.LogTrace("添加从站({Id})", unitId);

            OnAdded(slave);
            return slave;
        });
    }

    public ValueTask RemoveSalveAsync(byte unitId, CancellationToken cancellationToken)
    {
        return _stateMachine.ExecuteAsync(async _ =>
        {
            await _slaves.RemoveAsync(unitId, cancellationToken);
            _logger.LogTrace("删除从站({Id})", unitId);

            OnRemoved(unitId);

        }, cancellationToken);
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);




    protected virtual void OnAdded(ModbusSimulationSlave slave) { }
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