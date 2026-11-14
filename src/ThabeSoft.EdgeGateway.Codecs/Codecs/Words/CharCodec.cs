namespace ThabeSoft.EdgeGateway.Codecs.Words;

internal sealed record class CharCodec(ICodec<ushort> Codec) : ICodec<char>
{
    public int RequiredBits => Codec.RequiredBits;

    public int Encode(char value, Span<byte> buffer) => Codec.Encode(value, buffer);
    public char Decode(ReadOnlySpan<byte> raw) => (char)Codec.Decode(raw);

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((char)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}