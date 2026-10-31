namespace EdgeGateway.Byte.Codecs;


public interface IByteCodec<T> : IByteCodec
    where T : notnull
{
    int Encode(T value, Span<byte> buffer);
    new T Decode(ReadOnlySpan<byte> raw);
}

public interface IByteCodec
{
    int Encode(object value, Span<byte> buffer);
    object Decode(ReadOnlySpan<byte> raw);
}