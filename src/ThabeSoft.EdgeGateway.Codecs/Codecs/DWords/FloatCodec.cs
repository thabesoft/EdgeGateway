namespace ThabeSoft.EdgeGateway.Codecs.DWords;


internal sealed record class FloatCodec(ICodec<uint> Codec) : ICodec<float>
{
    public int RequiredBits => Codec.RequiredBits;

    public int Encode(float value, Span<byte> buffer) => Codec.Encode(BitConverter.SingleToUInt32Bits(value), buffer);
    public float Decode(ReadOnlySpan<byte> raw) => BitConverter.UInt32BitsToSingle(Codec.Decode(raw));

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((uint)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}