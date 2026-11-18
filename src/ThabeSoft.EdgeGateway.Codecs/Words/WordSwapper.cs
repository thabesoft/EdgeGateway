namespace ThabeSoft.EdgeGateway.Codecs.Words;


internal sealed class WordSwapper : ISwapper
{
    public static WordSwapper BigEndian { get; } = new(WordEndianess.Big);
    public static WordSwapper LittleEndian { get; } = new(WordEndianess.Little);

    public static WordSwapper Create(WordEndianess endianess)
    {
        return endianess switch
        {
            WordEndianess.Big => BigEndian,
            WordEndianess.Little => LittleEndian,
            _ => throw new ArgumentOutOfRangeException(nameof(endianess), endianess, "不支持的 Word 布局")
        };
    }


    private readonly WordEndianess _endianess;
    private WordSwapper(WordEndianess endianess) => _endianess = endianess;


    public const int RequiredBytes = 2;


    public void Swap(Span<byte> bytes)
    {
        if (bytes.Length != RequiredBytes)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        if (_endianess == WordEndianess.Little) SwapByte(bytes);
    }

    private static void SwapByte(Span<byte> bytes)
    {
        if (bytes.Length != RequiredBytes)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        (bytes[0], bytes[1]) = (bytes[1], bytes[0]);
    }
}