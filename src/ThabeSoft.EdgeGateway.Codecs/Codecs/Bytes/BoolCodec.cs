namespace ThabeSoft.EdgeGateway.Codecs.Bytes;


public sealed record class BoolCodec : ICodec<bool>
{
    private readonly int _bitOffset;
    private BoolCodec(int bitOffset)
    {
        if (bitOffset is < 0 or > 7)
            throw new ArgumentOutOfRangeException(nameof(bitOffset), "位偏移必须在 0~7。");

        _bitOffset = bitOffset;
    }

    public static ICodec<bool> BitOffset(int bitOffset) =>
        new BoolCodec(bitOffset);



    public const int RequiredBytes = 1;
    public int RequiredBits { get; } = RequiredBytes;



    public int Encode(bool value, Span<byte> buffer)
    {
        if (buffer.Length < RequiredBytes)
            throw new ArgumentException($"缓冲区至少需要 {RequiredBytes} 字节。", nameof(buffer));

        var mask = (byte)(1 << _bitOffset);
        if (value)
            buffer[0] |= mask;    // 置位
        else
            buffer[0] &= (byte)~mask;   // 清位

        return RequiredBytes;
    }

    public bool Decode(ReadOnlySpan<byte> raw)
    {
        if (raw.Length < RequiredBytes)
            throw new ArgumentException($"数据至少需要 {RequiredBytes} 字节。", nameof(raw));

        return (raw[0] & (1 << _bitOffset)) != 0;
    }

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((bool)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}