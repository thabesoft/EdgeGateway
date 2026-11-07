namespace ThabeSoft.EdgeGateway.Transport;


internal sealed class TransportConnectionFactorySelector(IEnumerable<ITransportConnectionFactory> factories)
{
    public ITransportConnection Create(ITransportDescriptor transportDescriptor)
    {
        return Select(transportDescriptor).Create(transportDescriptor);
    }

    public ITransportConnectionFactory Select(ITransportDescriptor transportDescriptor)
    {
        return factories.FirstOrDefault(x => x.CanCreate(transportDescriptor))
            ?? throw new InvalidOperationException($"无法创建端口, 不支持的描述: {transportDescriptor}");
    }
}