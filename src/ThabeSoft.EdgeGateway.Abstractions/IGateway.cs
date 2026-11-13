using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;


public interface IGateway : ILifecycleWithState
{
    IReadOnlyDictionary<ConnectionEndpoint, IPort> Ports { get; }

    ValueTask<IPort> AddPortAsync(PortDescriptor descriptor, CancellationToken cancellationToken = default);
    ValueTask<bool> RemovePortAsync(ConnectionEndpoint endpoint, CancellationToken cancellationToken = default);
}