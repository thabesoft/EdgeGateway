using System.Collections;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Codecs.Bits;


public sealed record class BitsCodecDescriptor : CodecDescriptor<BitArray>, IRequiredBits
{
    public required BitOrder BitOrder { get; init => field = value.Defined(); }
    public required int RequiredBits { get; init => field = value.Positive(); }
}