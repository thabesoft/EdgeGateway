using ThabeSoft.EdgeGateway.Recoveries;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


public sealed class PortDescriptor
{
    public PortDescriptor(ITransportDescriptor transport, IRecoveryPolicyDescriptor[] recoveries)
    {
        ArgumentNullException.ThrowIfNull(transport);

        Transport = transport;
        Recoveries = recoveries?.Where(x => x is not null).ToArray() ?? [];
    }

    public ITransportDescriptor Transport { get; init; }
    public IRecoveryPolicyDescriptor[] Recoveries { get; init; }
}
