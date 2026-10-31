namespace ThabeSoft.EdgeGateway.Byte.Swappers;


public sealed class WordSwapper : IByteSwapper
{
    public static WordSwapper BigEndian { get; } = new(false);
    public static WordSwapper LittleEndian { get; } = new(true);


    private const int ByteLength = 2;
    private readonly bool _isSwapByte;
    private WordSwapper(bool isWwapByte) => _isSwapByte = isWwapByte;

    public void Swap(Span<byte> bytes)
    {
        if (bytes.Length != ByteLength)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        if(_isSwapByte) SwapByte(bytes);
    }

    private static void SwapByte(Span<byte> bytes)
    {
        if (bytes.Length != ByteLength)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        (bytes[0], bytes[1]) = (bytes[1], bytes[0]);
    }
}