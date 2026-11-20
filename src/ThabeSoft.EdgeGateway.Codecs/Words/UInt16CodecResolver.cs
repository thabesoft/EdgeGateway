using ThabeSoft.EdgeGateway.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Codecs.Words;


internal sealed class UInt16CodecResolver : ICodecResolver<ushort>
{
    public bool CanResolve(CodecDescriptor<ushort> descriptor) => true;

    public ICodec<ushort> Resolve(CodecDescriptor<ushort> descriptor)
    {
        var opts = descriptor.AssertType<UInt16CodecDescriptor>();
        return WordCodec.Create(opts.Endianess);
    }
}
