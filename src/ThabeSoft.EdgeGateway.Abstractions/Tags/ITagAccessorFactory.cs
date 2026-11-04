using ThabeSoft.EdgeGateway.Transport.Byte;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Tags;

public interface ITagAccessorFactory
{
    bool CanCreate(IProtocolDescriptor descriptor);
    ITagAccessor Create(IProtocolDescriptor descriptor, IByteDuplex byteStream);
}