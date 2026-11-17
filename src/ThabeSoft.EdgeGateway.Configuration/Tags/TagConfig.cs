using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Subscriptions;

public sealed class TagConfig
{
    public required TagDescriptor Descriptor { get; init => field = value.Required(); }
    public required TagValueType ValueType { get; init => field = value.Required(); }
}

public enum TagValueType
{
    Bool, Byte,
    Int16, UInt16, Char,
    Int32, UInt32, Float,
    Int64, UInt64, Double,

    Bits,
    String
}