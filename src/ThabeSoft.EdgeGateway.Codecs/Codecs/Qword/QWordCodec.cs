using System.Buffers.Binary;

namespace ThabeSoft.EdgeGateway.Codecs.Qword;


public sealed record class QWordCodec : ICodec<ulong>
{
    public static ICodec<ulong> BigEndian { get; } = new QWordCodec(QWordSwapper.BigEndian);
    public static ICodec<ulong> LittleEndian { get; } = new QWordCodec(QWordSwapper.LittleEndian);


    private readonly ISwapper _swapper;

    private QWordCodec(ISwapper swapper)
    {
        ArgumentNullException.ThrowIfNull(swapper);

        if (swapper.RequiredBits != RequiredBits)
            throw new ArgumentOutOfRangeException($"数据交换器可处理数据长度与编码器所需长度不一致, 需要: {RequiredBytes} bytes", swapper.RequiredBytes, nameof(swapper));
        _swapper = swapper;
    }
    public static ICodec<ulong> From(params IEnumerable<QWordSwap> swaps)
    {
        var swapper = QWordSwapper.From(swaps);
        return new QWordCodec(swapper);
    }



    public const int RequiredBytes = 8;
    public int RequiredBits { get; } = RequiredBytes * 8;


    public int Encode(ulong value, Span<byte> buffer)
    {
        if (buffer.Length < RequiredBytes)
            throw new ArgumentException($"缓冲区至少需要 {RequiredBytes} 字节。", nameof(buffer));

        BinaryPrimitives.WriteUInt64BigEndian(buffer, value);
        _swapper.Swap(buffer);

        return RequiredBytes;
    }

    public ulong Decode(ReadOnlySpan<byte> raw)
    {
        if (raw.Length < RequiredBytes)
            throw new ArgumentException($"数据至少需要 {RequiredBytes} 字节。", nameof(raw));

        Span<byte> cache = stackalloc byte[RequiredBytes];
        raw.CopyTo(cache);
        _swapper.Swap(cache);

        return BinaryPrimitives.ReadUInt64BigEndian(cache);
    }

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((ulong)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}
