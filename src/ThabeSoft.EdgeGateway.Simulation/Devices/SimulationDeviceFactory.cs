namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class SimulationDeviceFactory(IEnumerable<ISimulationDeviceResolver> factories)
{
    public ISimulationDevice Create(SimulationDeviceDescriptor descriptor)
    {
        var factory = factories.FirstOrDefault(x => x.CanResolve(descriptor))
            ?? throw new InvalidOperationException("没有可创建的工厂");

        return factory.Resolve(descriptor);
    }
}