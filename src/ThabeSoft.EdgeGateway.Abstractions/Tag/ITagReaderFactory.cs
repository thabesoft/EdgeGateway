using ThabeSoft.EdgeGateway.Byte;
using ThabeSoft.EdgeGateway.Protocol;

namespace ThabeSoft.EdgeGateway.Tag;


public interface ITagReaderFactory
{
    bool CanCreate(IProtocolDescriptor descriptor);
    ITagReader Create(IProtocolDescriptor descriptor, IByteStream byteStream);
}