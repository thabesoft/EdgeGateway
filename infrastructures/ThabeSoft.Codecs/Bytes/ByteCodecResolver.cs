using ThabeSoft.Primitives;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.Codecs.Bytes;


internal sealed class ByteCodecResolver : ICodecResolver<byte>
{
    public bool CanResolve(CodecDescriptor<byte> descriptor)
    {
        return descriptor.DataType == DataType.Byte;
    }

    public ICodec<byte> Resolve(CodecDescriptor<byte> descriptor)
    {
        descriptor.AssertType<ByteCodecDescriptor>();
        return ByteCodec.Instance;
    }
}