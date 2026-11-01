namespace ThabeSoft.EdgeGateway.Codecs.Swappers;


public sealed class QWordSwapper : ISwapper
{
    public static QWordSwapper BigEndian { get; } = new([]);
    public static QWordSwapper LittleEndian { get; } = new([QWordSwap.DWord, QWordSwap.Word, QWordSwap.Byte]);

    public static QWordSwapper Create(IReadOnlyList<QWordSwap> swaps)
    {
        ArgumentNullException.ThrowIfNull(swaps);

        if (swaps.Any(x => x is not (QWordSwap.DWord or QWordSwap.Word or QWordSwap.Byte)))
            throw new ArgumentOutOfRangeException(nameof(swaps), "存在未定义的交换值");

        return new(swaps);
    }


    private const int ByteLength = 8;
    private readonly IReadOnlyList<QWordSwap> _swaps = [];
    private QWordSwapper(IReadOnlyList<QWordSwap> swaps) => _swaps = swaps;


    public void Swap(Span<byte> bytes)
    {
        if (bytes.Length != ByteLength)
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
        if (bytes.Length != ByteLength)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        (bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7]) =
        (bytes[4], bytes[5], bytes[6], bytes[7], bytes[0], bytes[1], bytes[2], bytes[3]);
    }
    private static void SwapWord(Span<byte> bytes)
    {
        if (bytes.Length != ByteLength)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        (bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7]) =
        (bytes[2], bytes[3], bytes[0], bytes[1], bytes[6], bytes[7], bytes[4], bytes[5]);
    }

    private static void SwapByte(Span<byte> bytes)
    {
        if (bytes.Length != ByteLength)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        (bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7]) =
        (bytes[1], bytes[0], bytes[3], bytes[2], bytes[5], bytes[4], bytes[7], bytes[6]);
    }
}

public enum QWordSwap
{
    Byte,
    Word,
    DWord,
}