using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Simulation.Networks;


public sealed class ModbusSimulatorNetworkDescriptor
{
    public required ITransportDescriptor Transport { get; init; }
    public required IProtocolDescriptor Protocol { get; init; }

    private ModbusSimulatorNetworkDescriptor() { }
    public override string ToString() => $"{Protocol}({Transport})";


    public static ModbusSimulatorNetworkDescriptor Create(ITransportDescriptor transportDescriptor, IProtocolDescriptor protocolDescriptor)
    {
        ArgumentNullException.ThrowIfNull(transportDescriptor);
        return new() { Protocol = protocolDescriptor, Transport = transportDescriptor };
    }
}