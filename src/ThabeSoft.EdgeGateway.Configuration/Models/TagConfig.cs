using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Configuration.Models;


public sealed class TagConfig
{
    public required TagDescriptor Descriptor { get; init => field = value.Required(); }
    public required CodecKind ValueType { get; init => field = value.Required(); }
}