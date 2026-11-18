namespace ThabeSoft.EdgeGateway.Devices;


internal interface ISimulationDeviceResolver
{
    bool CanResolve(SimulationDeviceCreationContext context);
    ISimulationDevice Resolve(SimulationDeviceCreationContext context);
}
