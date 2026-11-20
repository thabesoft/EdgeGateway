using ThabeSoft.EdgeGateway.DataTypes;
using ThabeSoft.EdgeGateway.Primitives.Guard;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.Configuration.Models;


public sealed class TagConfig
{
    public required ITagDescriptor Descriptor { get; init => field = value.Required(); }
    public required DataType Kind { get; init => field = value.Required(); }
}