using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Simulation.Networks;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Simulation;


public interface ISimulator : ILifecycle
{
    IModbusSimulationNetwork AddModbusNetwork(ModbusSimulatorNetworkDescriptor simulatorDescriptor);
}


internal sealed class Simulator(
        ModbusSimulationNetworkFactory simulationNetworkFactory,
        ILogger<Simulator>? logger
    ) : ISimulator, IAsyncDisposable
{
    private readonly ConcurrentDictionary<ConnectionEndpoint, IModbusSimulationNetwork> _networks = [];
    private readonly IRetryableLifecycleStateMachine _stateMachine = Lifecycle.RetryableStateMachine(logger);
    private readonly ILifecycleBatchExecutor _batchExecutor = Lifecycle.BatchExecutor(logger);

    public LifecycleState State => _stateMachine.State;


    public IModbusSimulationNetwork AddModbusNetwork(ModbusSimulatorNetworkDescriptor simulatorDescriptor)
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