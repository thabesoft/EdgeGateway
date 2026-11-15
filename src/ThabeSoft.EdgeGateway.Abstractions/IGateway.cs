using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway;


public interface IGateway : ILifecycleWithState
{
    IReadOnlyDictionary<PortId, IPort> Ports { get; }

    ValueTask<IPort> AddPortAsync(PortDescriptor descriptor, CancellationToken cancellationToken = default);
    ValueTask<bool> RemovePortAsync(PortId id, CancellationToken cancellationToken = default);
}