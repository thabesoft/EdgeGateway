using System.Buffers.Binary;

namespace ThabeSoft.EdgeGateway.Codecs.Internal.Qword;


internal sealed class QWordCodec(QWordSwapper qwordSwapper) : ICodec<ulong>, IRequiredBits
{
    public static QWordCodec BigEndian { get; } = new(QWordSwapper.BigEndian);
    public static QWordCodec LittleEndian { get; } = new(QWordSwapper.LittleEndian);

    public const int ByteLength = 8;
    public int RequiredBits { get; } = ByteLength * 8;


    public int Encode(ulong value, Span<byte> buffer)
    {
        if (buffer.Length < ByteLength)
            throw new ArgumentException($"缓冲区至少需要 {ByteLength} 字节。", nameof(buffer));

        BinaryPrimitives.WriteUInt64BigEndian(buffer, value);
        qwordSwapper.Swap(buffer);

        return ByteLength;
    }

    public ulong Decode(ReadOnlySpan<byte> raw)
    {
        if (raw.Length < ByteLength)
            throw new ArgumentException($"数据至少需要 {ByteLength} 字节。", nameof(raw));

        Span<byte> cache = stackalloc byte[ByteLength];
        raw.CopyTo(cache);
        qwordSwapper.Swap(cache);

        return BinaryPrimitives.ReadUInt64BigEndian(cache);
    }

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((ulong)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}
