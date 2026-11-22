using ThabeSoft.Protocols.Accessor;
using ThabeSoft.Transports.Byte;

namespace ThabeSoft.EdgeGateway.Protocols;


internal sealed class ProtocolAccessorFactory(IEnumerable<IProtocolAccessorResolver> resolvers) : IProtocolAccessorFactory
{
    public IProtocolAccessor Create(ProtocolDescriptor descriptor, IByteDuplex byteDuplex)
    {
        var resolver = resolvers.FirstOrDefault(x => x.CanResolve(descriptor))
            ?? throw new InvalidOperationException($"不支持创建的协议({descriptor.GetType()}):{descriptor}");

        return resolver.Resolve(descriptor, byteDuplex);
    }
}