using ThabeSoft.EdgeGateway.IO;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Tags;


public interface ITagReaderFactory
{
    bool CanCreate(IProtocolDescriptor descriptor);
    ITagReader Create(IProtocolDescriptor descriptor, IByteStream byteStream);
}