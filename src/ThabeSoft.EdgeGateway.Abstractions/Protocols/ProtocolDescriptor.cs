using ThabeSoft.EdgeGateway.Compatibility;

namespace ThabeSoft.EdgeGateway.Protocols;

public abstract record ProtocolDescriptor : IEquatable<ProtocolDescriptor>
{
    public abstract ProtocolType Type { get; }
}