using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Protocols;

public interface IProtocolAccessorFactory
{
    bool CanCreate(ProtocolDescriptor protocolDescriptor);
    IProtocolAccessor Create(ProtocolDescriptor protocolDescriptor, IByteDuplex byteStream);
}