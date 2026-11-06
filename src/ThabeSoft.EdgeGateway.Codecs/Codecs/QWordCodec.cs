using System.Buffers.Binary;
using ThabeSoft.EdgeGateway.Codecs.Swappers;

namespace ThabeSoft.EdgeGateway.Codecs;


public sealed class QWordCodec(QWordSwapper qwordSwapper) : ICodec<ulong>, IRequiredBits
{
    public static QWordCodec BigEndian { get; } = new(QWordSwapper.BigEndian);
    public static QWordCodec LittleEndian { get; } = new(QWordSwapper.LittleEndian);

    public const int RequiredBytes = 8;
    public int RequiredBits { get; } = RequiredBytes * 8;


    public int Encode(ulong value, Span<byte> buffer)
    {
        if (buffer.Length < RequiredBytes)
            throw new ArgumentException($"缓冲区至少需要 {RequiredBytes} 字节。", nameof(buffer));

        BinaryPrimitives.WriteUInt64BigEndian(buffer, value);
        qwordSwapper.Swap(buffer);

        return RequiredBytes;
    }

    public ulong Decode(ReadOnlySpan<byte> raw)
    {
        if (raw.Length < RequiredBytes)
            throw new ArgumentException($"数据至少需要 {RequiredBytes} 字节。", nameof(raw));

        Span<byte> cache = stackalloc byte[RequiredBytes];
        raw.CopyTo(cache);
        qwordSwapper.Swap(cache);

        return BinaryPrimitives.ReadUInt64BigEndian(cache);
    }

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((ulong)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}
