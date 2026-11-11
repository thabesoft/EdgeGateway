using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;


internal sealed class Simulator(
        SimulationPortFactorySelector factorySelector,
        ILogger<Simulator>? logger
    ) : ISimulator, IAsyncDisposable
{
    private readonly ConcurrentDictionary<ConnectionEndpoint, ISimulationPort> _ports = [];
    private readonly IRetryableLifecycleStateMachine _stateMachine = Lifecycle.RetryableStateMachine(logger);
    private readonly ILifecycleBatchExecutor _batchExecutor = Lifecycle.BatchExecutor(logger);

    public LifecycleState State => _stateMachine.State;


    public ISimulationPort AddPort(SimulatorPortDescriptor simulatorDescriptor)
    {
        return _stateMachine.Execute
        (
            action: () =>
            {
                var key = simulatorDescriptor.Transport.Endpoint;

                if (_ports.ContainsKey(key))
                    throw new InvalidOperationException("已存在");

                var simulator = factorySelector.Create(simulatorDescriptor);
                _ports[key] = simulator;

                return simulator;
            }
        );
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(ct => _batchExecutor.StartAsync(_ports.Values, ct), cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(ct => _batchExecutor.StopAsync(_ports.Values, ct), cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(() => _batchExecutor.DisposeAsync(_ports.Values));
}