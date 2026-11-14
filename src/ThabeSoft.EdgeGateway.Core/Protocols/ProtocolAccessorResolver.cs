using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Protocols;


internal sealed class ProtocolAccessorResolver(IEnumerable<IProtocolAccessorFactory> factories)
{
    public IProtocolAccessor Resolve(ProtocolDescriptor protocolDescriptor, IByteDuplex byteDuplex)
    {
        var factory = factories
            .FirstOrDefault(x => x.CanCreate(protocolDescriptor))
            ?? throw new InvalidOperationException("没有能执行的工厂");

        return factory.Create(protocolDescriptor, byteDuplex);
    }
}