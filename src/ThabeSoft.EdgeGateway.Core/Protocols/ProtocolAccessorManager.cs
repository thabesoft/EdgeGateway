using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Protocols;


internal sealed class ProtocolAccessorManager(IEnumerable<IProtocolAccessorFactory> factories)
{
    public IProtocolAccessor Register(IProtocolDescriptor descriptor, IByteDuplex byteDuplex)
    {
        return (factories.FirstOrDefault(x => x.CanCreate(descriptor))
            ?.Create(descriptor, byteDuplex))
            ?? throw new InvalidOperationException("没有工厂能创建");
    }
}