namespace ThabeSoft.EdgeGateway.Codecs.Internal.Qword;

internal sealed class LongCodec<TQWordCodec>(TQWordCodec codec) : ICodec<long>, IRequiredBits
    where TQWordCodec : ICodec<ulong>, IRequiredBits
{
    public int RequiredBits => codec.RequiredBits;

    public int Encode(long value, Span<byte> buffer) => codec.Encode(unchecked((ulong)value), buffer);
    public long Decode(ReadOnlySpan<byte> raw) => (long)codec.Decode(raw);

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((short)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}
