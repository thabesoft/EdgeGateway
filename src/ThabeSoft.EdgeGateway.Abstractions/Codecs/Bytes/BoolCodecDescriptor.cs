using ThabeSoft.EdgeGateway.Primitives;
using ThabeSoft.EdgeGateway.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Codecs.Bytes;


public sealed record class BoolCodecDescriptor : CodecDescriptor<bool>, IRequiredBits
{
    public int BitOffset { get; init => field = value.InRange(0, 7); }
    public int RequiredBits { get; } = 8;
}