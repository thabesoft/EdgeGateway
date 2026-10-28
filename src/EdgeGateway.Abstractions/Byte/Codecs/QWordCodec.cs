using EdgeGateway.Byte.Swappers;
using System.Buffers.Binary;

namespace EdgeGateway.Byte.Codecs;


public sealed class QWordCodec(QWordSwapper qwordSwapper) : IByteCodec<ulong>
{
    public static QWordCodec BigEndian { get; } = new(QWordSwapper.BigEndian);
    public static QWordCodec LittleEndian { get; } = new(QWordSwapper.LittleEndian);


    public const int ByteLength = 8;


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

    int IByteCodec.Encode(object value, Span<byte> buffer) => Encode((ulong)value, buffer);
    object IByteCodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}