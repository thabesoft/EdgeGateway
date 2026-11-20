namespace ThabeSoft.EdgeGateway.Codecs.DWords;


internal sealed class DWordSwapper : ISwapper
{
    public static DWordSwapper Abcd { get; } = new DWordSwapper(DWordLayout.Abcd);
    public static DWordSwapper Badc { get; } = new DWordSwapper(DWordLayout.Badc);
    public static DWordSwapper Cdab { get; } = new DWordSwapper(DWordLayout.Cdab);
    public static DWordSwapper Dcba { get; } = new DWordSwapper(DWordLayout.Dcba);


    private readonly DWordLayout _layout;
    private DWordSwapper(DWordLayout layout) => _layout = layout;


    private const int ByteLength = 4;


    public void Swap(Span<byte> bytes)
    {
        if (bytes.Length != ByteLength)
            throw new ArgumentException("数据长度不足, 至少需要 4 字节", nameof(bytes));


        if(_layout == DWordLayout.Abcd)
        {
            return;
        }
        if (_layout == DWordLayout.Badc)
        {
            SwapByte(bytes);
        }
        if (_layout == DWordLayout.Cdab)
        {
            SwapWord(bytes);
        }
        if(_layout == DWordLayout.Dcba)
        {
            SwapWord(bytes);
            SwapByte(bytes);
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