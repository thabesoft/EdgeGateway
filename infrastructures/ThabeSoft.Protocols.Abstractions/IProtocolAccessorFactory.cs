using ThabeSoft.Protocols;
using ThabeSoft.Transports.Byte;

namespace ThabeSoft.EdgeGateway.Protocols;

public interface IProtocolAccessorFactory
{
    IProtocolAccessor Create(ProtocolDescriptor descriptor, IByteDuplex byteDuplex);
}
