using EdgeGateway.Byte;
using EdgeGateway.Protocol;

namespace EdgeGateway.Tag;


public interface ITagWriterFactory
{
    bool CanCreate(IProtocolDescriptor descriptor);
    ITagWriter Create(IProtocolDescriptor descriptor, IByteStream byteStream);
}