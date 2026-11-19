using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Primitives;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Simulation;


internal sealed class Simulator(in SimulatorArgs args) : ISimulator, IAsyncDisposable
{
    private readonly ConcurrentDictionary<ConnectionEndpoint, ISimulationPort> _ports = [];
    private readonly ILifecycleStateMachine _stateMachine = args.LifecycleStateMachineFactory.Create(args.SimulatorStateMachine);
    private readonly ILifecycleBatchExecutor _portBatchExecutor = args.LifecycleBatchExecutorFactory.Create(args.PortBatchExecutor);
    private readonly SimulationPortFactory _portFactory = args.PortFactory;

    private readonly LifecycleStateMachineDescriptor _portStateMachine = args.PortStateMachine;
    private readonly LifecycleStateMachineDescriptor _deviceStateMachine = args.DeviceStateMachine;
    private readonly LifecycleBatchExecutorDescriptor _deviceDeviceBatchExecutor = args.DeviceBatchExecutor;



    public LifecycleState State => _stateMachine.State;
    public IReadOnlyCollection<ISimulationPort> Ports => [.. _ports.Values];


    public ISimulationPort AddPort(SimulatorPortDescriptor simulatorDescriptor)
    {
        return _stateMachine.Execute
        (
            handler: () =>
            {
                var key = simulatorDescriptor.Transport.Endpoint;

                if (_ports.ContainsKey(key))
                    throw new InvalidOperationException("已存在");

                var context = new SimulatorPortCreationContext()
                {
                    Port = simulatorDescriptor,
                    DeviceBatchExecutor = _deviceDeviceBatchExecutor,
                    PortStateMachine = _portStateMachine,
                    DeviceStateMachine = _deviceStateMachine,
                };

                var simulator = _portFactory.Create(context);
                _ports[key] = simulator;

                return simulator;
            }
        );
    }
    public ValueTask RemovePortAsync(ConnectionEndpoint endpoint, CancellationToken cancellationToken)
    {
        return _stateMachine.ExecuteAsync(async ct =>
        {
            if (_ports.TryRemove(endpoint, out var port))
                await port.DisposeAsync();

        }, cancellationToken);
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.TransitionToRunningAsync(ct => _portBatchExecutor.StartBatchAsync(_ports.Values, ct), cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.TransitionToRunningAsync(ct => _portBatchExecutor.StopBatchAsync(_ports.Values, ct), cancellationToken);

    public async ValueTask DisposeAsync()
    {
        // TODO : 没有幂等释放

        await Safe.DisposeSafelyAsync(_stateMachine);
        await Safe.DisposeSafelyBatchAsync(_ports.Values);
    }
}