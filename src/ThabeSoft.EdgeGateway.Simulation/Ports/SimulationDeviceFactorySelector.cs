namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class SimulationPortFactorySelector(IEnumerable<ISimulationPortFactory> factories)
{
    public ISimulationPort Create(SimulatorPortDescriptor descriptor)
    {
        var factory = factories.FirstOrDefault(x => x.CanCreate(descriptor))
            ?? throw new InvalidOperationException("没有可创建的工厂");

        return factory.Create(descriptor);
    }
}