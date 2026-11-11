namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class SimulationDeviceFactorySelector(IEnumerable<ISimulationDeviceFactory> factories)
{
    public ISimulationDevice Create(ISimulationDeviceDescriptor descriptor)
    {
        var factory = factories.FirstOrDefault(x => x.CanCreate(descriptor))
            ?? throw new InvalidOperationException("没有可创建的工厂");

        return factory.Create(descriptor);
    }
}