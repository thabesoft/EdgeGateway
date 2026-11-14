using System.Collections.Immutable;

namespace ThabeSoft.EdgeGateway.Codecs.Qword;


internal sealed record class QWordSwapper : ISwapper
{
    public static ISwapper BigEndian { get; } = new QWordSwapper([]);
    public static ISwapper LittleEndian { get; } = new QWordSwapper([QWordSwap.DWord, QWordSwap.Word, QWordSwap.Byte]);


    private readonly ImmutableArray<QWordSwap> _swaps = [];

    private QWordSwapper(IEnumerable<QWordSwap> swaps)
    {
        ArgumentNullException.ThrowIfNull(swaps);

        if (swaps.Any(x => x is not (QWordSwap.DWord or QWordSwap.Word or QWordSwap.Byte)))
            throw new ArgumentOutOfRangeException(nameof(swaps), "存在未定义的交换值");

        _swaps = [.. swaps];
    }
    public static ISwapper From(params IEnumerable<QWordSwap> swaps) =>
        new QWordSwapper(swaps);


    private const int RequiredBytes = 8;
    public int RequiredBits { get; } = RequiredBytes * 8;


    public void Swap(Span<byte> bytes)
    {
        if (bytes.Length != RequiredBytes)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        foreach (var i in _swaps)
        {
            switch (i)
            {
                case QWordSwap.Byte:
                    SwapByte(bytes);
                    break;
                case QWordSwap.Word:
                    SwapWord(bytes);
                    break;
                case QWordSwap.DWord:
                    SwapDWord(bytes);
                    break;
            }
        }
    }
    private static void SwapDWord(Span<byte> bytes)
    {
        if (bytes.Length != RequiredBytes)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        (bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7]) =
        (bytes[4], bytes[5], bytes[6], bytes[7], bytes[0], bytes[1], bytes[2], bytes[3]);
    }
    private static void SwapWord(Span<byte> bytes)
    {
        if (bytes.Length != RequiredBytes)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        (bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7]) =
        (bytes[2], bytes[3], bytes[0], bytes[1], bytes[6], bytes[7], bytes[4], bytes[5]);
    }
    private static void SwapByte(Span<byte> bytes)
    {
        if (bytes.Length != RequiredBytes)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        (bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7]) =
        (bytes[1], bytes[0], bytes[3], bytes[2], bytes[5], bytes[4], bytes[7], bytes[6]);
    }
}