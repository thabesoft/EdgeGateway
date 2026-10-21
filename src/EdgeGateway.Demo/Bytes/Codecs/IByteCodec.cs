namespace EdgeGateway.Bytes.Codecs;


public interface IByteCodec<T>
{
    int Encode(T value, Span<byte> buffer);
    T Decode(ReadOnlySpan<byte> raw);
}