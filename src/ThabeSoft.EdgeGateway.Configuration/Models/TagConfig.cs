using ThabeSoft.Primitives.Guard;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.Primitives;

namespace ThabeSoft.EdgeGateway.Configuration.Models;


public sealed class TagConfig
{
    public required ITagDescriptor Descriptor { get; init => field = value.Required(); }
    public required DataType Kind { get; init => field = value.Required(); }
}