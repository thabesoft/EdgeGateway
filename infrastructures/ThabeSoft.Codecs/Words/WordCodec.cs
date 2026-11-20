using System.Buffers.Binary;

namespace ThabeSoft.EdgeGateway.Codecs.Words;


public sealed class WordCodec : ICodec<ushort>
{
    public static WordCodec BigEndian { get; } = new WordCodec(WordSwapper.BigEndian);
    public static WordCodec LittleEndian { get; } = new WordCodec(WordSwapper.LittleEndian);


    public static WordCodec Create(WordEndianess endianess)
    {
        return endianess switch
        {
            WordEndianess.Big => BigEndian,
            WordEndianess.Little => LittleEndian,
            _ => throw new ArgumentOutOfRangeException(nameof(endianess), endianess, "不支持的 Word 布局")
        };
    }


    private readonly ISwapper _swapper;
    private WordCodec(WordSwapper swapper) => _swapper = swapper;


    public const int RequiredBytes = 2;


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
}


internal sealed class ShortCodec(ICodec<ushort> Codec) : ICodec<short>
{
    public int Encode(short value, Span<byte> buffer) => Codec.Encode(unchecked((ushort)value), buffer);
    public short Decode(ReadOnlySpan<byte> raw) => (short)Codec.Decode(raw);
}


internal sealed class CharCodec(ICodec<ushort> Codec) : ICodec<char>
{
    public int Encode(char value, Span<byte> buffer) => Codec.Encode(value, buffer);
    public char Decode(ReadOnlySpan<byte> raw) => (char)Codec.Decode(raw);
}