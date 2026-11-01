using System.Buffers.Binary;
using ThabeSoft.EdgeGateway.Codecs.Swappers;

namespace ThabeSoft.EdgeGateway.Codecs;


public sealed class WordCodec : ICodec<ushort>
{
    public static WordCodec BigEndian { get; } = new(WordSwapper.BigEndian);
    public static WordCodec LittleEndian { get; } = new(WordSwapper.LittleEndian);


    public const int ByteLength = 2;
    private readonly ISwapper _byteSwapper;
    private WordCodec(ISwapper byteSwapper) => _byteSwapper = byteSwapper;


    public int Encode(ushort value, Span<byte> buffer)
    {
        if (buffer.Length < ByteLength)
            throw new ArgumentException($"缓冲区至少需要 {ByteLength} 字节。", nameof(buffer));

        BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
        _byteSwapper.Swap(buffer);

        return 2;
    }

    public ushort Decode(ReadOnlySpan<byte> raw)
    {
        if (raw.Length < ByteLength)
            throw new ArgumentException($"数据至少需要 {ByteLength} 字节。", nameof(raw));

        Span<byte> cache = stackalloc byte[ByteLength];
        raw.CopyTo(cache);
        _byteSwapper.Swap(cache);

        return (ushort)((cache[0] << 8) | cache[1]);
    }

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((ushort)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}