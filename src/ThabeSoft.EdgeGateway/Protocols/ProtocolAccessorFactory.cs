using ThabeSoft.EdgeGateway.Transports.Byte;

namespace ThabeSoft.EdgeGateway.Protocols;


internal sealed class ProtocolAccessorFactory(IEnumerable<IProtocolAccessorResolver> resolvers)
{
    public IProtocolAccessor Create(ProtocolDescriptor descriptor, IByteDuplex byteDuplex)
    {
        var factory = resolvers.FirstOrDefault(x => x.CanResolve(descriptor))
            ?? throw new NotSupportedProtocolException(descriptor);

        return factory.Resolve(descriptor, byteDuplex);
    }
}