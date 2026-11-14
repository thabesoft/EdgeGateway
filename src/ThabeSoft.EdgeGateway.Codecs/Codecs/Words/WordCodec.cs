using System.Buffers.Binary;

namespace ThabeSoft.EdgeGateway.Codecs.Words;


public sealed record class WordCodec : ICodec<ushort>
{
    public static ICodec<ushort> BigEndian { get; } = new WordCodec(WordSwapper.BigEndian);
    public static ICodec<ushort> LittleEndian { get; } = new WordCodec(WordSwapper.LittleEndian);


    private readonly ISwapper _swapper;
    private WordCodec(ISwapper swapper)
    {
        ArgumentNullException.ThrowIfNull(swapper);

        if (swapper.RequiredBits != RequiredBits)
            throw new ArgumentOutOfRangeException($"数据交换器可处理数据长度与编码器所需长度不一致, 需要: {RequiredBytes} bytes", swapper.RequiredBytes, nameof(swapper));
        _swapper = swapper;
    }


    public const int RequiredBytes = 2;
    public int RequiredBits { get; } = RequiredBytes * 8;


    public int Encode(ushort value, Span<byte> buffer)
    {
        if (buffer.Length < RequiredBytes)
            throw new ArgumentException($"缓冲区至少需要 {RequiredBytes} 字节。", nameof(buffer));

        BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
        _swapper.Swap(buffer);

        return 2;
    }

    public ushort Decode(ReadOnlySpan<byte> raw)
    {
        if (raw.Length < RequiredBytes)
            throw new ArgumentException($"数据至少需要 {RequiredBytes} 字节。", nameof(raw));

        Span<byte> cache = stackalloc byte[RequiredBytes];
        raw.CopyTo(cache);
        _swapper.Swap(cache);

        return (ushort)((cache[0] << 8) | cache[1]);
    }

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((ushort)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}