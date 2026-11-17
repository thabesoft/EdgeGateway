namespace ThabeSoft.EdgeGateway.Transports;


internal sealed class TransportConnectionFactory(IEnumerable<ITransportConnectionResolver> resolvers) : ITransportConnectionFactoryFactory
{
    public ITransportConnection Resolve(TransportDescriptor transportDescriptor)
    {
        var resolver = resolvers.FirstOrDefault(x => x.CanResolve(transportDescriptor))
            ?? throw new InvalidOperationException($"无法创建通讯连接, 不支持的描述: {transportDescriptor}");

        return resolver.Resolve(transportDescriptor);
    }
}