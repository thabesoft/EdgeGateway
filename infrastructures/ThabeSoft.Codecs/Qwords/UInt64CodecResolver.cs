using ThabeSoft.EdgeGateway.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Codecs.QWords;


internal sealed class UInt64CodecResolver : ICodecResolver<ulong>
{
    public bool CanResolve(CodecDescriptor<ulong> descriptor) => true;

    public ICodec<ulong> Resolve(CodecDescriptor<ulong> descriptor)
    {
        var opts = descriptor.AssertType<UInt64CodecDescriptor>();
        return QWordCodec.Create(opts.Swaps);
    }
}