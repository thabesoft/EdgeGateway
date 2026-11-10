using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;


public interface IGateway : ILifecycleWithState
{
    IReadOnlyDictionary<ConnectionEndpoint, IPort> Ports { get; }
    ValueTask<IPort> AddPortAsync(ITransportDescriptor transportDescriptor, CancellationToken cancellationToken = default);
    ValueTask<bool> RemovePortAsync(ConnectionEndpoint connectionEndpoint, CancellationToken cancellationToken = default);
}