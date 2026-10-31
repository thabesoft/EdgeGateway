using ThabeSoft.EdgeGateway.Byte;
using ThabeSoft.EdgeGateway.Protocol;

namespace ThabeSoft.EdgeGateway.Tag;


public interface ITagWriterFactory
{
    bool CanCreate(IProtocolDescriptor descriptor);
    ITagWriter Create(IProtocolDescriptor descriptor, IByteStream byteStream);
}