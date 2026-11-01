using ThabeSoft.EdgeGateway.Codecs;

namespace ThabeSoft.EdgeGateway.Tags;


public interface ITag<T> : ITag
    where T : notnull
{
    new ICodec<T> Codec { get; }

#if NETCOREAPP
    ICodec ITag.Codec => Codec;
#endif
}

public interface ITag
{
    IDataAddress Address { get; }
    ICodec Codec { get; }
}