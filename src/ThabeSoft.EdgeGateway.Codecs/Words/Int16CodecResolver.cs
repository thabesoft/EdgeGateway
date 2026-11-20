using ThabeSoft.EdgeGateway.Codecs.Extensions;
using ThabeSoft.EdgeGateway.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Codecs.Words;

internal sealed class Int16CodecResolver : ICodecResolver<short>
{
    public bool CanResolve(CodecDescriptor<short> descriptor) => true;

    public ICodec<short> Resolve(CodecDescriptor<short> descriptor)
    {
        var opts = descriptor.AssertType<UInt16CodecDescriptor>();
        return WordCodec.Create(opts.Endianess).AsShort();
    }
}
