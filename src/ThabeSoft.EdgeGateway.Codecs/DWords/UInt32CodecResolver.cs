using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Codecs.DWords;


internal sealed class UInt32CodecResolver : ICodecResolver<uint>
{
    public bool CanResolve(CodecDescriptor<uint> descriptor) => true;

    public ICodec<uint> Resolve(CodecDescriptor<uint> descriptor)
    {
        var opts = descriptor.AssertType<UInt32CodecDescriptor>();
        return DWordCodec.Create(opts.Layout);
    }
}
