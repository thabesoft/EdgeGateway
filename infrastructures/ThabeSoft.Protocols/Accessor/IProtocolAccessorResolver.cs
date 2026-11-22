using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.Transports.Byte;

namespace ThabeSoft.Protocols.Accessor;


public interface IProtocolAccessorResolver
{
    bool CanResolve(ProtocolDescriptor protocolDescriptor);
    IProtocolAccessor Resolve(ProtocolDescriptor protocolDescriptor, IByteDuplex byteStream);
}