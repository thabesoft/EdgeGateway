using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


internal abstract class SimulationPortBase : ISimulationPort
{
    private readonly ILifecycleKeyedCollection<ISimulationDeviceId, ISimulationDevice> _devices;
    private readonly IRetryableLifecycleStateMachine _stateMachine;
    private readonly ILifecycleBatchExecutor _batchExecutor;
    private readonly SimulationDeviceFactorySelector _deviceFactorySelector;
    private readonly ILogger _logger;


    protected SimulationPortBase(ConnectionEndpoint endpoint, SimulationDeviceFactorySelector deviceFactorySelector, ILogger? logger = null)
    {
        _deviceFactorySelector = deviceFactorySelector;
        _logger = logger ?? NullLogger.Instance;


        _batchExecutor = Lifecycle.BatchExecutor(logger);
        _stateMachine = Lifecycle.RetryableStateMachine(logger);
        _devices = Lifecycle.KeyedCollection<ISimulationDeviceId, ISimulationDevice>(_stateMachine, logger);

        Endpoint = endpoint;
    }

    public LifecycleState State => _stateMachine.State;
    public ConnectionEndpoint Endpoint { get; }

    public ValueTask<ISimulationDevice> AddDeviceAsync(SimulationDeviceDescriptor simulationDeviceDescriptor, CancellationToken cancellationToken = default)
    {
        return _stateMachine.ExecuteAsync(async ct =>
        {
            var slave = await _devices.AddAsync(simulationDeviceDescriptor.Id, () => _deviceFactorySelector.Create(simulationDeviceDescriptor), ct);
            _logger.LogTrace("添加从站({Id})", simulationDeviceDescriptor.Id);

            OnAdded(slave);
            return slave;

        }, cancellationToken);
    }
    public ValueTask RemoveDeviceAsync(ISimulationDeviceId id, CancellationToken cancellationToken = default)
    {
        return _stateMachine.ExecuteAsync(async _ =>
        {
            await _devices.RemoveAsync(id, cancellationToken);
            _logger.LogTrace("删除从站({Id})", id);

            OnRemoved(id);

        }, cancellationToken);
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);
    public async void Dispose() =>
         DisposeAsync().AsTask().GetAwaiter().GetResult();



    protected virtual void OnAdded(ISimulationDevice device) { }
    protected virtual void OnRemoved(ISimulationDeviceId deviceId) { }

    protected virtual ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        return _batchExecutor.StartAsync(_devices.Values, cancellationToken);
    }
    protected virtual ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        return _batchExecutor.StopAsync(_devices.Values, cancellationToken);
    }
    protected virtual ValueTask OnDisposeAsync()
    {
        return _batchExecutor.DisposeAsync(_devices.Values);
    }

   
}