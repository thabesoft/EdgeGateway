namespace ThabeSoft.EdgeGateway.Codecs.Words;

internal sealed class CharCodec<TWordCodec>(TWordCodec codec) : ICodec<char>, IRequiredBits
    where TWordCodec : ICodec<ushort>, IRequiredBits
{
    public int RequiredBits => codec.RequiredBits;

    public int Encode(char value, Span<byte> buffer) => codec.Encode(value, buffer);
    public char Decode(ReadOnlySpan<byte> raw) => (char)codec.Decode(raw);

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((char)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}