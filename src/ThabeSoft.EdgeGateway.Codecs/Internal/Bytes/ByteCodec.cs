namespace ThabeSoft.EdgeGateway.Codecs.Internal.Bytes;


internal sealed class ByteCodec : ICodec<byte>, IRequiredBits
{
    private ByteCodec() { }
    public static ByteCodec Instance { get; } = new();


    public const int RequiredBytes = 1;
    public int RequiredBits { get; } = RequiredBytes * 8;



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

    int ICodec.Encode(object value, Span<byte> buffer) => Encode((byte)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}