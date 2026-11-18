using System.Buffers.Binary;
using ThabeSoft.EdgeGateway.Codecs.Words;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Codecs.QWords;


public sealed class QWordCodec : ICodec<ulong>
{
    public static QWordCodec BigEndian { get; } = new QWordCodec(QWordSwapper.BigEndian);
    public static QWordCodec LittleEndian { get; } = new QWordCodec(QWordSwapper.LittleEndian);

    public static QWordCodec Create(params IEnumerable<QWordSwap> swaps)
    {
        var swapper = QWordSwapper.Create(swaps);
        return new QWordCodec(swapper);
    }


    private readonly ISwapper _swapper;
    private QWordCodec(QWordSwapper swapper) => _swapper = swapper;



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
}



internal sealed class DoubleCodec(ICodec<ulong> codec) : ICodec<double>
{
    public int Encode(double value, Span<byte> buffer) =>
        codec.Encode(BitConverter.DoubleToUInt64Bits(value), buffer);
    public double Decode(ReadOnlySpan<byte> raw) =>
        BitConverter.UInt64BitsToDouble(codec.Decode(raw));
}

internal sealed class LongCodec(ICodec<ulong> Codec) : ICodec<long>
{
    public int Encode(long value, Span<byte> buffer) => Codec.Encode(unchecked((ulong)value), buffer);
    public long Decode(ReadOnlySpan<byte> raw) => (long)Codec.Decode(raw);
}