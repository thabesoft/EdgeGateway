using ThabeSoft.EdgeGateway.IO;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Tags;


public interface ITagWriterFactory
{
    bool CanCreate(IProtocolDescriptor descriptor);
    ITagWriter Create(IProtocolDescriptor descriptor, IByteStream byteStream);
}