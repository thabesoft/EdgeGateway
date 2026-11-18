using ThabeSoft.EdgeGateway.DataTypes;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Configuration.Models;


public sealed class TagConfig
{
    public required TagDescriptor Descriptor { get; init => field = value.Required(); }
    public required DataType Kind { get; init => field = value.Required(); }
}