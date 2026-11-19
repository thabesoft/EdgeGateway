using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Ports;


internal abstract class SimulationPortBase : ISimulationPort
{
    private readonly ILifecycleKeyedCollection<ISimulationDeviceId, ISimulationDevice> _devices;
    private readonly ILifecycleStateMachine _stateMachine;
    private readonly ILifecycleBatchExecutor _batchExecutor;
    private readonly SimulationDeviceFactory _deviceFactory;
    private readonly ILogger _logger;
    private readonly LifecycleStateMachineDescriptor _deviceStateMachine;


    protected SimulationPortBase(in SimulationPortBaseArgs args)
    {
        _deviceFactory = args.DeviceFactory;
        _logger = args.Logger;

        _batchExecutor = args.LifecycleBatchExecutorFactory.Create(args.DeviceBatchExecutor);
        _stateMachine = args.LifecycleStateMachineFactory.Create(args.PortStateMachine);
        _devices = args.LifecycleKeyedCollectionFactory.Create<ISimulationDeviceId, ISimulationDevice>(_stateMachine);
        _deviceStateMachine = args.DeviceStateMachine;

        Endpoint = args.Transport.Endpoint;
    }

    public LifecycleState State => _stateMachine.State;
    public ConnectionEndpoint Endpoint { get; }

    public ValueTask<ISimulationDevice> AddDeviceAsync(SimulationDeviceDescriptor simulationDeviceDescriptor, CancellationToken cancellationToken = default)
    {
        return _stateMachine.ExecuteAsync(async ct =>
        {
            var slave = await _devices.AddAsync(simulationDeviceDescriptor.Id, () => _deviceFactory.Create(new() { Device = simulationDeviceDescriptor, DeviceStateMachine = _deviceStateMachine }), ct);
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
        return _batchExecutor.StartBatchAsync(_devices.Values, cancellationToken);
    }
    protected virtual ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        return _batchExecutor.StopBatchAsync(_devices.Values, cancellationToken);
    }
    protected virtual async ValueTask OnDisposeAsync()
    {
        await SafeDisposeExtensions.DisposeSafelyAsync(_batchExecutor);
    }
}