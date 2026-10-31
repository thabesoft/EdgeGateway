using EdgeGateway.Byte.Codecs;

namespace EdgeGateway.Tag;


public interface ITag<T> : ITag
    where T : notnull
{
    new IByteCodec<T> Codec { get; }

#if NETCOREAPP
    IByteCodec ITag.Codec => Codec;
#endif
}

public interface ITag
{
    IDataAddress Address { get; }
    IByteCodec Codec { get; }
}