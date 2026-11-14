namespace ThabeSoft.EdgeGateway.Codecs.Qword;


internal sealed record class DoubleCodec(ICodec<ulong> codec) : ICodec<double>
{
    public int RequiredBits => codec.RequiredBits;

    public int Encode(double value, Span<byte> buffer) =>
        codec.Encode(BitConverter.DoubleToUInt64Bits(value), buffer);
    public double Decode(ReadOnlySpan<byte> raw) =>
        BitConverter.UInt64BitsToDouble(codec.Decode(raw));

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((double)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}