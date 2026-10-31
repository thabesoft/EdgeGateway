using EdgeGateway.Byte;
using EdgeGateway.Protocol;

namespace EdgeGateway.Tag;


public interface ITagReaderFactory
{
    bool CanCreate(IProtocolDescriptor descriptor);
    ITagReader Create(IProtocolDescriptor descriptor, IByteStream byteStream);
}