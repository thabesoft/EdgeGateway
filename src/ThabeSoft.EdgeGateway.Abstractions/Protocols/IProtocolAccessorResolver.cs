using ThabeSoft.Transports.Byte;

namespace ThabeSoft.EdgeGateway.Protocols;


public interface IProtocolAccessorResolver
{
    bool CanResolve(ProtocolDescriptor protocolDescriptor);
    IProtocolAccessor Resolve(ProtocolDescriptor protocolDescriptor, IByteDuplex byteStream);
}