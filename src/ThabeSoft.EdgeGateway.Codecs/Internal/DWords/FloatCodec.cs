namespace ThabeSoft.EdgeGateway.Codecs.Internal.DWords;

internal sealed class FloatCodec<TDWordCodec>(TDWordCodec codec) : ICodec<float>, IRequiredBits
    where TDWordCodec : ICodec<uint>, IRequiredBits
{
    public int RequiredBits => codec.RequiredBits;

    public int Encode(float value, Span<byte> buffer) => codec.Encode(BitConverter.SingleToUInt32Bits(value), buffer);
    public float Decode(ReadOnlySpan<byte> raw) => BitConverter.UInt32BitsToSingle(codec.Decode(raw));

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((uint)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}