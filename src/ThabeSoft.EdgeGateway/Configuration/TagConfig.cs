using ThabeSoft.Primitives;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.Protocols;

namespace ThabeSoft.EdgeGateway.Configuration;


public sealed class TagConfig
{
    public required IDataAccessDescriptor Descriptor { get; init => field = value.Required(); }
    public required DataType Kind { get; init => field = value.Required(); }
}