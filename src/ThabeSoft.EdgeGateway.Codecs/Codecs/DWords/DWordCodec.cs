using System.Buffers.Binary;

namespace ThabeSoft.EdgeGateway.Codecs.DWords;


public sealed record class DWordCodec : ICodec<uint>
{
    public static ICodec<uint> BigEndian { get; } = new DWordCodec(DWordSwapper.BigEndian);
    public static ICodec<uint> LittleEndian { get; } = new DWordCodec(DWordSwapper.LittleEndian);

    public static ICodec<uint> ABCD { get; } = BigEndian;
    public static ICodec<uint> DCBA { get; } = LittleEndian;
    public static ICodec<uint> CDAB { get; } = new DWordCodec(DWordSwapper.CDAB);
    public static ICodec<uint> BADC { get; } = new DWordCodec(DWordSwapper.BADC);


    private readonly ISwapper _swapper;
    private DWordCodec(ISwapper swapper)
    {
        if (swapper.RequiredBits != RequiredBits)
            throw new ArgumentOutOfRangeException($"数据交换器可处理数据长度与编码器所需长度不一致, 需要: {ByteLength} bytes", swapper.RequiredBytes, nameof(swapper));
        _swapper = swapper;
    }


    public const int ByteLength = 4;
    public int RequiredBits { get; } = ByteLength * 8;



    public int Encode(uint value, Span<byte> buffer)
    {
        if (buffer.Length < ByteLength)
            throw new ArgumentException($"缓冲区至少需要 {ByteLength} 字节。", nameof(buffer));

        BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
        _swapper.Swap(buffer);

        return ByteLength;
    }

    public uint Decode(ReadOnlySpan<byte> raw)
    {
        if (raw.Length < ByteLength)
            throw new ArgumentException($"数据至少需要 {ByteLength} 字节。", nameof(raw));

        Span<byte> cache = stackalloc byte[ByteLength];
        raw.CopyTo(cache);
        _swapper.Swap(cache);

        return BinaryPrimitives.ReadUInt32BigEndian(cache);
    }

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((uint)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}