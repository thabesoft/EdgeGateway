namespace ThabeSoft.EdgeGateway.Devices;


public interface ISimulationDeviceFactory
{
    bool CanCreate(ISimulationDeviceDescriptor descriptor);
    ISimulationDevice Create(ISimulationDeviceDescriptor descriptor);
}
