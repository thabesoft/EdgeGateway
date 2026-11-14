namespace ThabeSoft.EdgeGateway.Codecs.Words;


internal sealed record class ShortCodec(ICodec<ushort> Codec) : ICodec<short>
{
    public int RequiredBits => Codec.RequiredBits;

    public int Encode(short value, Span<byte> buffer) => Codec.Encode(unchecked((ushort)value), buffer);
    public short Decode(ReadOnlySpan<byte> raw) => (short)Codec.Decode(raw);

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((short)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}
