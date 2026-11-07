using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Protocols;


internal sealed class ProtocolAccessorManager(IEnumerable<IProtocolAccessorFactory> factories)
{
    public IProtocolAccessor Register(IProtocolDescriptor descriptor, IByteDuplex byteDuplex)
    {
        var factory = factories.FirstOrDefault(x => x.CanCreate(descriptor))
            ?? throw new InvalidOperationException($"不支持的描述符 {descriptor.GetType()} 没有工厂能创建");

        return factory.Create(descriptor, byteDuplex);
    }
}