
using System.Collections;
using ThabeSoft.EdgeGateway.DataTypes;
using ThabeSoft.EdgeGateway.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Codecs.Bits;


internal sealed class BitsCodecResolver : ICodecResolver<BitArray>
{
    public bool CanResolve(CodecDescriptor<BitArray> descriptor)
    {
        return descriptor.DataType == DataType.Bits;
    }

    public ICodec<BitArray> Resolve(CodecDescriptor<BitArray> descriptor)
    {
        var bits = descriptor.AssertType<BitsCodecDescriptor>();
        return BitsCodec.Bits(bits.RequiredBits, bits.BitOrder);
    }
}