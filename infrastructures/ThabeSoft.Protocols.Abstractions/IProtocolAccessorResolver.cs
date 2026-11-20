using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.Transports.Byte;

namespace ThabeSoft.Protocols;


public interface IProtocolAccessorResolver
{
    bool CanResolve(ProtocolDescriptor protocolDescriptor);
    IProtocolAccessor Resolve(ProtocolDescriptor protocolDescriptor, IByteDuplex byteStream);
}