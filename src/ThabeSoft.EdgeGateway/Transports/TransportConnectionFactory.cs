namespace ThabeSoft.EdgeGateway.Transports;


internal sealed class TransportConnectionFactory(
        IEnumerable<ITransportConnectionResolver> resolvers
    ) : ITransportConnectionFactory
{
    public ITransportConnection Create(TransportDescriptor descriptor)
    {
        var resolver = resolvers.FirstOrDefault(x => x.CanResolve(descriptor))
            ?? throw new InvalidOperationException($"不支持创建的传输类型({descriptor.GetType()}):{descriptor}");

        return resolver.Resolve(descriptor);
    }
}