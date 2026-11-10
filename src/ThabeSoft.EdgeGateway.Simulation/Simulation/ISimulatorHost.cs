using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Simulation.Networks;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Simulation;


public interface ISimulatorHost : ILifecycle
{
    IModbusSimulationNetwork AddNetwork(ModbusSimulatorNetworkDescriptor simulatorDescriptor);
}

internal sealed class SimulatorHost(
        ModbusSimulationNetworkFactory simulationNetworkFactory
    ) : ISimulatorHost, IAsyncDisposable
{
    private readonly ConcurrentDictionary<ConnectionEndpoint, IModbusSimulationNetwork> _networks = [];
    private readonly IRetryableLifecycleStateMachine _stateMachine = Lifecycle.RetryableStateMachine();
    private readonly ILifecycleBatchExecutor _batchExecutor = Lifecycle.BatchExecutor();

    public LifecycleState State => _stateMachine.State;


    public IModbusSimulationNetwork AddNetwork(ModbusSimulatorNetworkDescriptor simulatorDescriptor)
    {
        return _stateMachine.Execute
        (
            action: () =>
            {
                var key = simulatorDescriptor.Transport.Endpoint;

                if (_networks.ContainsKey(key))
                    throw new InvalidOperationException("已存在");

                var simulator = simulationNetworkFactory.Create(simulatorDescriptor);
                _networks[key] = simulator;

                return simulator;
            }
        );
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(ct => _batchExecutor.StartAsync(_networks.Values, ct), cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(ct => _batchExecutor.StopAsync(_networks.Values, ct), cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(() => _batchExecutor.DisposeAsync(_networks.Values));

}