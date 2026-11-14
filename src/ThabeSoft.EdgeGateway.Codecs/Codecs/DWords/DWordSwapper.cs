using System.Collections.Immutable;

namespace ThabeSoft.EdgeGateway.Codecs.DWords;


internal sealed record class DWordSwapper : ISwapper
{
    public static ISwapper BigEndian { get; } = new DWordSwapper([]);
    public static ISwapper LittleEndian { get; } = new DWordSwapper([DWordSwap.Word, DWordSwap.Byte]);

    public static ISwapper ABCD { get; } = BigEndian;
    public static ISwapper DCBA { get; } = LittleEndian;
    public static ISwapper CDAB { get; } = new DWordSwapper([DWordSwap.Word]);
    public static ISwapper BADC { get; } = new DWordSwapper([DWordSwap.Byte]);


    private readonly ImmutableArray<DWordSwap> _swaps;
    private DWordSwapper(IEnumerable<DWordSwap> swaps)
    {
        _swaps = [.. swaps];
    }


    private const int ByteLength = 4;
    public int RequiredBits { get; } = ByteLength * 8;


    public void Swap(Span<byte> bytes)
    {
        if (bytes.Length != ByteLength)
            throw new ArgumentException("数据长度不足, 至少需要 4 字节", nameof(bytes));

        foreach (var i in _swaps)
        {
            switch (i)
            {
                case DWordSwap.Byte:
                    SwapByte(bytes);
                    break;
                case DWordSwap.Word:
                    SwapWord(bytes);
                    break;
            }
        }
    }

    private static void SwapWord(Span<byte> bytes)
    {
        if (bytes.Length != ByteLength)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        (bytes[0], bytes[1], bytes[2], bytes[3]) =
        (bytes[2], bytes[3], bytes[0], bytes[1]);
    }

    private static void SwapByte(Span<byte> bytes)
    {
        if (bytes.Length != ByteLength)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        (bytes[0], bytes[1], bytes[2], bytes[3]) =
        (bytes[1], bytes[0], bytes[3], bytes[2]);
    }
}