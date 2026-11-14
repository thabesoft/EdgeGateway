namespace ThabeSoft.EdgeGateway.Codecs.DWords;


internal sealed record class IntCodec(ICodec<uint> Codec) : ICodec<int>
{
    public int RequiredBits => Codec.RequiredBits;

    public int Encode(int value, Span<byte> buffer) => Codec.Encode((uint)value, buffer);
    public int Decode(ReadOnlySpan<byte> raw) => (int)Codec.Decode(raw);

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((int)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}
