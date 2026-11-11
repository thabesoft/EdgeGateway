namespace ThabeSoft.EdgeGateway.Devices;


public interface ISimulationDeviceFactory
{
    bool CanCreate(SimulationDeviceDescriptor descriptor);
    ISimulationDevice Create(SimulationDeviceDescriptor descriptor);
}
