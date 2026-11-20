using ThabeSoft.Primitives;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.Codecs.Bytes;


internal sealed class BoolCodecResolver : ICodecResolver<bool>
{
    public bool CanResolve(CodecDescriptor<bool> descriptor)
    {
        return descriptor.DataType == DataType.Bool;
    }

    public ICodec<bool> Resolve(CodecDescriptor<bool> descriptor)
    {
        var @bool = descriptor.AssertType<BoolCodecDescriptor>();
        return BoolCodec.BitOffset(@bool.BitOffset);
    }
}