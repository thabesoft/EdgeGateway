using EdgeGateway.Byte.Swappers;
using System.Buffers.Binary;

namespace EdgeGateway.Byte.Codecs;


public sealed class DWordCodec : IByteCodec<uint>
{
    public static DWordCodec BigEndian { get; } = new(DWordSwapper.BigEndian);
    public static DWordCodec LittleEndian { get; } = new(DWordSwapper.LittleEndian);

    public static DWordCodec ABCD { get; } = BigEndian;
    public static DWordCodec DCBA { get; } = LittleEndian;
    public static DWordCodec CDAB { get; } = new(DWordSwapper.CDAB);
    public static DWordCodec BADC { get; } = new(DWordSwapper.BADC);


    public const int ByteLength = 4;
    private readonly DWordSwapper _wordSwapper;
    private DWordCodec(DWordSwapper wordSwapper) => _wordSwapper = wordSwapper;



    public int Encode(uint value, Span<byte> buffer)
    {
        if (buffer.Length < ByteLength)
            throw new ArgumentException($"缓冲区至少需要 {ByteLength} 字节。", nameof(buffer));

        BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
        _wordSwapper.Swap(buffer);

        return ByteLength;
    }

    public uint Decode(ReadOnlySpan<byte> raw)
    {
        if (raw.Length < ByteLength)
            throw new ArgumentException($"数据至少需要 {ByteLength} 字节。", nameof(raw));

        Span<byte> cache = stackalloc byte[ByteLength];
        raw.CopyTo(cache);
        _wordSwapper.Swap(cache);

        return BinaryPrimitives.ReadUInt32BigEndian(cache);
    }

    int IByteCodec.Encode(object value, Span<byte> buffer) => Encode((uint)value, buffer);
    object IByteCodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}
