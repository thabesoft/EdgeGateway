using System.Buffers.Binary;

namespace ThabeSoft.EdgeGateway.Codecs.Words;


internal sealed class WordCodec : ICodec<ushort>, IRequiredBits
{
    public static WordCodec BigEndian { get; } = new(WordSwapper.BigEndian);
    public static WordCodec LittleEndian { get; } = new(WordSwapper.LittleEndian);


    private readonly ISwapper _byteSwapper;
    private WordCodec(ISwapper byteSwapper) => _byteSwapper = byteSwapper;


    public const int RequiredBytes = 2;
    public int RequiredBits { get; } = RequiredBytes * 8;


    public int Encode(ushort value, Span<byte> buffer)
    {
        if (buffer.Length < RequiredBytes)
            throw new ArgumentException($"缓冲区至少需要 {RequiredBytes} 字节。", nameof(buffer));

        BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
        _byteSwapper.Swap(buffer);

        return 2;
    }

    public ushort Decode(ReadOnlySpan<byte> raw)
    {
        if (raw.Length < RequiredBytes)
            throw new ArgumentException($"数据至少需要 {RequiredBytes} 字节。", nameof(raw));

        Span<byte> cache = stackalloc byte[RequiredBytes];
        raw.CopyTo(cache);
        _byteSwapper.Swap(cache);

        return (ushort)((cache[0] << 8) | cache[1]);
    }

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((ushort)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}