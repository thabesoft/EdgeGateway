namespace ThabeSoft.EdgeGateway.Transports;


internal sealed class TransportConnectionFactory(
        IEnumerable<ITransportConnectionResolver> resolvers
    ) : ITransportConnectionFactory
{
    public ITransportConnection Create(TransportCreationContext context)
    {
        var resolver = resolvers.FirstOrDefault(x => x.CanResolve(context))
            ?? throw new InvalidOperationException($"不支持创建的传输类型({context.Transport.GetType()}):{context.Transport}");

        return resolver.Resolve(context);
    }
}