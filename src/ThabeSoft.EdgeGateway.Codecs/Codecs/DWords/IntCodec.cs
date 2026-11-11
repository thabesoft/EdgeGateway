namespace ThabeSoft.EdgeGateway.Codecs.DWords;


internal sealed class IntCodec<TDWordCodec>(TDWordCodec codec) : ICodec<int>, IRequiredBits
    where TDWordCodec : ICodec<uint>, IRequiredBits
{
    public int RequiredBits => codec.RequiredBits;

    public int Encode(int value, Span<byte> buffer) => codec.Encode((uint)value, buffer);
    public int Decode(ReadOnlySpan<byte> raw) => (int)codec.Decode(raw);

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((int)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}
