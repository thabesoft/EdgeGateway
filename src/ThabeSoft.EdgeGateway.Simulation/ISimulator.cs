using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway;


public interface ISimulator : ILifecycle, ILifecycleStateable
{
    ISimulationPort AddPort(SimulatorPortDescriptor simulatorDescriptor);
}