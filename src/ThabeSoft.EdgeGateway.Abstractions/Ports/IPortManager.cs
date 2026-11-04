using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;

public interface IPortManager
{
    IReadOnlyDictionary<ConnectionEndpoint, IPort> Ports { get; }

    IPort AddPort(ITransportDescriptor transportDescriptor);
    ValueTask RemovePortAsync(ConnectionEndpoint id, CancellationToken cancellationToken = default);
}