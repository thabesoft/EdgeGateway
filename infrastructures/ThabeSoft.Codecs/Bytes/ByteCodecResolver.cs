using ThabeSoft.EdgeGateway.DataTypes;
using ThabeSoft.EdgeGateway.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Codecs.Bytes;


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