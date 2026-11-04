using ThabeSoft.EdgeGateway.Codecs;

namespace ThabeSoft.EdgeGateway.Tags;


public interface ITagDescriptor<T> : ITagDescriptor
    where T : notnull
{
    new ICodec<T> Codec { get; }

#if NETCOREAPP
    ICodec ITagDescriptor.Codec => Codec;
#endif
}

public interface ITagDescriptor
{
    IDataAddress Address { get; }
    ICodec Codec { get; }
}