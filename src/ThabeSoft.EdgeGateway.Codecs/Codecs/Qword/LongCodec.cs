namespace ThabeSoft.EdgeGateway.Codecs.Qword;


internal sealed record class LongCodec(ICodec<ulong> Codec) : ICodec<long>
{
    public int RequiredBits => Codec.RequiredBits;

    public int Encode(long value, Span<byte> buffer) => Codec.Encode(unchecked((ulong)value), buffer);
    public long Decode(ReadOnlySpan<byte> raw) => (long)Codec.Decode(raw);

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((short)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}
