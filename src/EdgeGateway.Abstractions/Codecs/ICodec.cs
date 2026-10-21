namespace EdgeGateway.Codecs;


/// <summary>
/// 数据反序列化接口
/// </summary>
public interface ICodec<T>
{
    T Decode(ReadOnlySpan<byte> bytes);
    int Encode(T value, Span<byte> buffer);
}



public sealed class ByteCodec : ICodec<byte>
{
    public byte Decode(ReadOnlySpan<byte> rawData)
    {
        if (rawData.Length < 1)
            throw new ArgumentException("Raw data is too short to decode a byte.", nameof(rawData));
        return rawData[0];
    }
    public int Encode(byte value, Span<byte> buffer)
    {
        if (buffer.Length < 1)
            throw new ArgumentException("Buffer is too small to encode a byte.", nameof(buffer));
        buffer[0] = value;
        return 1; // Number of bytes written
    }
}