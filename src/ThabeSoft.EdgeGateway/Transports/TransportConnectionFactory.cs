namespace ThabeSoft.EdgeGateway.Transports;


internal sealed class TransportConnectionFactory(
        IEnumerable<ITransportConnectionResolver> resolvers
    ) : ITransportConnectionFactory
{
    public ITransportConnection Create(TransportCreationContext context)
    {
        var resolver = resolvers.FirstOrDefault(x => x.CanResolve(context))
            ?? throw new InvalidOperationException($"无法创建通讯连接, 不支持的上下文: {context}");

        return resolver.Resolve(context);
    }
}