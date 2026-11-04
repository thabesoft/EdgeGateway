using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Protocols;

public interface IProtocolAccessorFactory
{
    bool CanCreate(IProtocolDescriptor protocolDescriptor);
    IProtocolAccessor Create(IProtocolDescriptor protocolDescriptor, IByteDuplex byteStream);
}