using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Codecs.Bytes;


internal sealed class ByteCodecResolver : ICodecResolver<byte>
{
    public bool CanResolve(CodecDescriptor<byte> descriptor)
    {
        return descriptor.Kind == CodecKind.Byte;
    }

    public ICodec<byte> Resolve(CodecDescriptor<byte> descriptor)
    {
        descriptor.AssertType<ByteCodecDescriptor>();
        return ByteCodec.Instance;
    }
}