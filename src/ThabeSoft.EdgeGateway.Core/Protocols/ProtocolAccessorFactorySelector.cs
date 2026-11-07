using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Protocols;


internal sealed class ProtocolAccessorFactorySelector(IEnumerable<IProtocolAccessorFactory> factories)
{
    public IProtocolAccessorFactory Select(IProtocolDescriptor protocolDescriptor)
    {
        return factories
            .FirstOrDefault(x => x.CanCreate(protocolDescriptor))
            ?? throw new InvalidOperationException("没有能执行的工厂");
    }

    public IProtocolAccessor Create(IProtocolDescriptor protocolDescriptor, IByteDuplex byteDuplex)
    {
        var factory = Select(protocolDescriptor);
        return factory.Create(protocolDescriptor, byteDuplex);
    }
}