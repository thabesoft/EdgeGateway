namespace ThabeSoft.EdgeGateway.Codecs.Words;

internal sealed class ShortCodec<TWordCodec>(TWordCodec codec) : ICodec<short>, IRequiredBits
    where TWordCodec : ICodec<ushort>, IRequiredBits
{
    public int RequiredBits => codec.RequiredBits;

    public int Encode(short value, Span<byte> buffer) => codec.Encode(unchecked((ushort)value), buffer);
    public short Decode(ReadOnlySpan<byte> raw) => (short)codec.Decode(raw);

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((short)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}
