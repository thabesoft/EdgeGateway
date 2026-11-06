namespace ThabeSoft.EdgeGateway.Codecs;

public sealed class IntCodec(DWordCodec codec) : ICodec<int>, IRequiredBits
{
    public int RequiredBits => codec.RequiredBits;

    public int Encode(int value, Span<byte> buffer) => codec.Encode((uint)value, buffer);
    public int Decode(ReadOnlySpan<byte> raw) => (int)codec.Decode(raw);

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((int)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}
