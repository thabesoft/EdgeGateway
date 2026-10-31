using System.Collections.Concurrent;

namespace ThabeSoft.EdgeGateway.Byte.Codecs;


public sealed class BoolCodec : IByteCodec<bool>
{
    private const int ByteLength = 1;
    private readonly int _bitOffset;
    private BoolCodec(int bitOffset) => _bitOffset = bitOffset;


    public int Encode(bool value, Span<byte> buffer)
    {
        if (buffer.Length < ByteLength)
            throw new ArgumentException($"缓冲区至少需要 {ByteLength} 字节。", nameof(buffer));

        var mask = (byte)(1 << _bitOffset);
        if (value)
            buffer[0] |= mask;    // 置位
        else
            buffer[0] &= (byte)~mask;   // 清位

        return ByteLength;
    }

    public bool Decode(ReadOnlySpan<byte> raw)
    {
        if (raw.Length < ByteLength)
            throw new ArgumentException($"数据至少需要 {ByteLength} 字节。", nameof(raw));

        return (raw[0] & (1 << _bitOffset)) != 0;
    }

    int IByteCodec.Encode(object value, Span<byte> buffer) => Encode((bool)value, buffer);
    object IByteCodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);



    public static BoolCodec BitOffset(int bitOffset)
    {
        if (bitOffset is < 0 or > 7)
            throw new ArgumentOutOfRangeException(nameof(bitOffset), "位偏移必须在 0~7。");

        return _cache.GetOrAdd(bitOffset, x => new BoolCodec(x));
    }

    private static readonly ConcurrentDictionary<int, BoolCodec> _cache = [];
}