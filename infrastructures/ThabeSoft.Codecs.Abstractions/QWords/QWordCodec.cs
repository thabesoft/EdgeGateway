using System.Collections.Immutable;

namespace ThabeSoft.Codecs.QWords;


public abstract record class QWordCodecDescriptor<TQWord> : CodecDescriptor<TQWord>, IRequiredBits
{
    static QWordCodecDescriptor()
    {
        var type = typeof(TQWord);
        if (type != typeof(ulong) && type != typeof(long) && type != typeof(double))
        {
            throw new InvalidOperationException($"类型 {type.Name} 不是有效的 QWord 类型(ulong/long/double)");
        }
    }

    public int RequiredBits { get; } = 8 * 8;
    public required ImmutableArray<QWordSwap> Swaps { get; init; }
}


public sealed record class UInt64CodecDescriptor : QWordCodecDescriptor<ulong>
{
    public static UInt64CodecDescriptor BigEndian { get; } = new() { Swaps = [] };
    // 12345678   (5678 1234)  (78 56 34 12)  (87653421)
    public static UInt64CodecDescriptor LittleEndian { get; } = new() { Swaps = [ QWordSwap.DWord, QWordSwap.Word, QWordSwap.Byte] };
}
public sealed record class Int64CodecDescriptor : QWordCodecDescriptor<long>
{
    public static Int64CodecDescriptor BigEndian { get; } = new() { Swaps = [] };
    public static Int64CodecDescriptor LittleEndian { get; } = new() { Swaps = [QWordSwap.DWord, QWordSwap.Word, QWordSwap.Byte] };
}
public sealed record class DoubleCodecDescriptor : QWordCodecDescriptor<double>
{
    public static DoubleCodecDescriptor BigEndian { get; } = new() { Swaps = [] };
    public static DoubleCodecDescriptor LittleEndian { get; } = new() { Swaps = [QWordSwap.DWord, QWordSwap.Word, QWordSwap.Byte] };
}