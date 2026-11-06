namespace ThabeSoft.EdgeGateway.Codecs;


public interface ICodec
{
    int Encode(object value, Span<byte> buffer);
    object Decode(ReadOnlySpan<byte> raw);
}

public interface ICodec<T> : ICodec
    where T : notnull
{
    int Encode(T value, Span<byte> buffer);
    new T Decode(ReadOnlySpan<byte> raw);
}