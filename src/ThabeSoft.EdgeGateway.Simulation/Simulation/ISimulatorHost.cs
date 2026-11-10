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
    private readonly CollectionLifecycleStateMachine _stateMachine = Lifecycle.CollectionStateMachine();
    private readonly ConcurrentDictionary<ConnectionEndpoint, IModbusSimulationNetwork> _networks = [];

    public LifecycleState State => _stateMachine.State;


    public IModbusSimulationNetwork AddNetwork(ModbusSimulatorNetworkDescriptor simulatorDescriptor)
    {
        return _stateMachine.Execute
        (
            state: (Items: _networks, Descriptor: simulatorDescriptor, NetworkFactory: simulationNetworkFactory),
            action: static state =>
            {
                var key = state.Descriptor.Transport.Endpoint;

                if (state.Items.ContainsKey(key))
                    throw new InvalidOperationException("已存在");

                var simulator = state.NetworkFactory.Create(state.Descriptor);
                state.Items[key] = simulator;

                return simulator;
            }
        );
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAllAsync([.. _networks.Values], cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAllAsync([.. _networks.Values], cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.StopAllAsync([.. _networks.Values]);

}