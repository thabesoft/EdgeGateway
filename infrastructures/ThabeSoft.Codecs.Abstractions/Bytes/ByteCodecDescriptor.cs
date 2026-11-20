namespace ThabeSoft.Codecs.Bytes;


public sealed record class ByteCodecDescriptor : CodecDescriptor<byte>, IRequiredBits
{
    private ByteCodecDescriptor() { }
    public static ByteCodecDescriptor Instance { get; } = new();

    public int RequiredBits { get; } = 8;
}