using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;


public interface ISimulator : ILifecycle, ILifecycleStateable
{
    IReadOnlyCollection<ISimulationPort> Ports { get; }
    ISimulationPort AddPort(SimulatorPortDescriptor simulatorDescriptor);
    void RemovePort(ConnectionEndpoint endpoint);
}