using ThabeSoft.EdgeGateway.Devices;

namespace ThabeSoft.EdgeGateway.Ports;


public interface ISimulationPortFactory
{
    bool CanCreate(SimulatorPortDescriptor descriptor);
    ISimulationPort Create(SimulatorPortDescriptor descriptor);
}
