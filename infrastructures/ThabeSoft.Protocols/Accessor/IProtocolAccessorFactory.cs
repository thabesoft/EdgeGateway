using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.Transports.Byte;

namespace ThabeSoft.Protocols.Accessor;

public interface IProtocolAccessorFactory
{
    IProtocolAccessor Create(ProtocolDescriptor descriptor, IByteDuplex byteDuplex);
}
