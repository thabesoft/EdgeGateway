using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


public sealed class SimulatorPortDescriptor
{
    public SimulatorPortDescriptor(ITransportDescriptor transport, IProtocolDescriptor protocol)
    {
        ArgumentNullException.ThrowIfNull(transport);
        ArgumentNullException.ThrowIfNull(protocol);

        Transport = transport;
        Protocol = protocol;
    }

    public ITransportDescriptor Transport { get; }
    public IProtocolDescriptor Protocol { get; }



    public override string ToString() => $"{Protocol}({Transport})";
}