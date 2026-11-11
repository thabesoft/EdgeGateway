using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway;


public interface ISimulator : ILifecycle
{
    ISimulationPort AddPort(SimulatorPortDescriptor simulatorDescriptor);
}