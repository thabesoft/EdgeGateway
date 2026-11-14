using System.Collections.Immutable;
using ThabeSoft.EdgeGateway.Compatibility;
using ThabeSoft.EdgeGateway.Recoveries;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


public sealed record class PortDescriptor
{
    public required TransportDescriptor Transport { get; init => field = value.Required(); }
    public ImmutableArray<IRecoveryPolicyDescriptor> Recoveries { get; init => field = value.ToImmutableArrayOrEmpty(); } = [];
}