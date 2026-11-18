using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Ports;


public interface ISimulationPortResolver
{
    bool CanCreate(SimulatorPortDescriptor descriptor);
    ISimulationPort Resolve(SimulatorPortDescriptor descriptor);
}


internal sealed class SimulatorPortDescriptorArgs
{
    public required SimulatorPortDescriptor Port { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor StateMachine { get; init => field = value.Required(); }

}