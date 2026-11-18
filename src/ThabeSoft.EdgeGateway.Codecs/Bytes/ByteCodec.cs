namespace ThabeSoft.EdgeGateway.Codecs.Bytes;


public sealed class ByteCodec : ICodec<byte>
{
    private ByteCodec() { }
    public static ICodec<byte> Instance { get; } = new ByteCodec();


    public const int RequiredBytes = 1;


    public int Encode(byte value, Span<byte> buffer)
    {
        if (buffer.Length < RequiredBytes)
            throw new ArgumentException($"缓冲区至少需要 {RequiredBytes} 字节。", nameof(buffer));

        buffer[0] = value;
        return RequiredBytes;
    }

    public byte Decode(ReadOnlySpan<byte> raw)
    {
        if (raw.Length < RequiredBytes)
            throw new ArgumentException($"数据至少需要 {RequiredBytes} 字节。", nameof(raw));

        return raw[0];
    }
}