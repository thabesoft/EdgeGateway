namespace ThabeSoft.EdgeGateway.Devices;


public interface ISimulationDeviceResolver
{
    bool CanResolve(SimulationDeviceDescriptor descriptor);
    ISimulationDevice Resolve(SimulationDeviceDescriptor descriptor);
}
