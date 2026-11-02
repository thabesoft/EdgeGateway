using ThabeSoft.EdgeGateway.IO;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Tags;

public interface ITagAccessor : ITagReader, ITagWriter;

public interface ITagAccessorFactory
{
    bool CanCreate(IProtocolDescriptor descriptor);
    ITagAccessor Create(IProtocolDescriptor descriptor, IByteStream byteStream);
}