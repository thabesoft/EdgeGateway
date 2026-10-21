using EdgeGateway.Bytes.Codecs;

namespace EdgeGateway.Tags;


public interface ITag<T>
{
    IDataAddress Address { get; }
    IByteCodec<T> Codec { get; }
}