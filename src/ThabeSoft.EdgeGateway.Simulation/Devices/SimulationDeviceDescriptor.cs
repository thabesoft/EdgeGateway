namespace ThabeSoft.EdgeGateway.Devices;

public sealed class SimulationDeviceDescriptor
{
    public SimulationDeviceDescriptor(ISimulationDeviceId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        Id = id;
    }

    public ISimulationDeviceId Id { get; }
}