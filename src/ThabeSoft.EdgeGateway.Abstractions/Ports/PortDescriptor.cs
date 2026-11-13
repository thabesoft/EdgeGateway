using ThabeSoft.EdgeGateway.Recoveries;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


public sealed class PortDescriptor : IEquatable<PortDescriptor>
{
    public PortDescriptor(ITransportDescriptor transport, params IRecoveryPolicyDescriptor[] recoveries)
    {
        ArgumentNullException.ThrowIfNull(transport);

        Transport = transport;
        Recoveries = recoveries?.Where(x => x is not null).ToArray() ?? [];
    }
    public PortDescriptor(ITransportDescriptor transport) : this(transport, [])
    {

    }


    public ITransportDescriptor Transport { get; init; }
    public IRecoveryPolicyDescriptor[] Recoveries { get; init; }


    public bool Equals(PortDescriptor other)
    {
        if (other is null) return false;
        if (!Transport.Equals(other.Transport)) return false;

        return Recoveries.SequenceEqual(other.Recoveries);
    }
}
