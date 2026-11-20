using ThabeSoft.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Transports;

namespace ThabeSoft.EdgeGateway;


public interface ISimulator : ILifecycle, ILifecycleStateable
{
    IReadOnlyCollection<ISimulationPort> Ports { get; }
    ISimulationPort AddPort(SimulatorPortDescriptor simulatorDescriptor);
    ValueTask RemovePortAsync(ConnectionEndpoint endpoint, CancellationToken cancellationToken = default);
}