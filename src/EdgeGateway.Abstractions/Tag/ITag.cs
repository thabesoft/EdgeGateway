using EdgeGateway.Byte.Codecs;

namespace EdgeGateway.Tag;


public interface ITag<T>
{
    IDataAddress Address { get; }
    IByteCodec<T> Codec { get; }
}