namespace ThabeSoft.EdgeGateway.Transport;


internal sealed class TransportConnectionResolver(IEnumerable<ITransportConnectionFactory> factories) : ITransportConnectionFactoryResolver
{
    public ITransportConnection Resolve(ITransportDescriptor transportDescriptor)
    {
        var factory = factories.FirstOrDefault(x => x.CanCreate(transportDescriptor))
            ?? throw new InvalidOperationException($"无法创建通讯连接, 不支持的描述: {transportDescriptor}");

        return factory.Create(transportDescriptor);
    }
}