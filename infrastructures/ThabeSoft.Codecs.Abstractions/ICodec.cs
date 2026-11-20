namespace ThabeSoft.EdgeGateway.Codecs;


public interface ICodec<T>
    where T : notnull
{
    int Encode(T value, Span<byte> buffer);
    T Decode(ReadOnlySpan<byte> raw);
}