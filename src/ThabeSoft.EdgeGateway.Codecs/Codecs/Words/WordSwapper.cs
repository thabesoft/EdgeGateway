namespace ThabeSoft.EdgeGateway.Codecs.Words;


internal sealed record class WordSwapper : ISwapper
{
    public static WordSwapper BigEndian { get; } = new(false);
    public static WordSwapper LittleEndian { get; } = new(true);


    private readonly bool _isSwapByte;
    private WordSwapper(bool isWwapByte) => _isSwapByte = isWwapByte;


    public const int RequiredBytes = 2;
    public int RequiredBits { get; } = RequiredBytes * 8;


    public void Swap(Span<byte> bytes)
    {
        if (bytes.Length != RequiredBytes)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        if(_isSwapByte) SwapByte(bytes);
    }

    private static void SwapByte(Span<byte> bytes)
    {
        if (bytes.Length != RequiredBytes)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        (bytes[0], bytes[1]) = (bytes[1], bytes[0]);
    }
}