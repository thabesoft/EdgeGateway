namespace ThabeSoft.EdgeGateway.Byte.Swappers;


public sealed class DWordSwapper : IByteSwapper
{
    public static DWordSwapper BigEndian { get; } = new([]);
    public static DWordSwapper LittleEndian { get; } = new([DWordSwap.Word, DWordSwap.Byte]);

    public static DWordSwapper ABCD { get; } = BigEndian;
    public static DWordSwapper DCBA { get; } = LittleEndian;
    public static DWordSwapper CDAB { get; } = new([DWordSwap.Word]);
    public static DWordSwapper BADC { get; } = new([DWordSwap.Byte]);

    private const int ByteLength = 4;
    private readonly IReadOnlyList<DWordSwap> _swaps = [];
    private DWordSwapper(IReadOnlyList<DWordSwap> swaps) => _swaps = swaps;


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

public enum DWordSwap
{
    Byte,
    Word,
}