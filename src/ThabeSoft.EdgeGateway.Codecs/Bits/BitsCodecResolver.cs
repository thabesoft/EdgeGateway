
using System.Collections;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Codecs.Bits;


internal sealed class BitsCodecResolver : ICodecResolver<BitArray>
{
    public bool CanResolve(CodecDescriptor<BitArray> descriptor)
    {
        return descriptor.Kind == CodecKind.Bits;
    }

    public ICodec<BitArray> Resolve(CodecDescriptor<BitArray> descriptor)
    {
        var bits = descriptor.AssertType<BitsCodecDescriptor>();
        return BitsCodec.Bits(bits.RequiredBits, bits.BitOrder);
    }
}