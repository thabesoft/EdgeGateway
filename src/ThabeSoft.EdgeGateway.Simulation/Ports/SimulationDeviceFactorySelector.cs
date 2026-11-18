namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class SimulationPortFactory(IEnumerable<ISimulationPortResolver> resolvers)
{
    public ISimulationPort Create(SimulatorPortDescriptor descriptor)
    {
        var factory = resolvers.FirstOrDefault(x => x.CanCreate(descriptor))
            ?? throw new InvalidOperationException("没有可创建的工厂");

        return factory.Resolve(descriptor);
    }
}