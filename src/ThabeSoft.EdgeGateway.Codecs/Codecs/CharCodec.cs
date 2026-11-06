namespace ThabeSoft.EdgeGateway.Codecs;

public sealed class CharCodec(WordCodec codec) : ICodec<char>, IRequiredBits
{
    public int RequiredBits => codec.RequiredBits;

    public int Encode(char value, Span<byte> buffer) => codec.Encode(value, buffer);
    public char Decode(ReadOnlySpan<byte> raw) => (char)codec.Decode(raw);

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((char)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}