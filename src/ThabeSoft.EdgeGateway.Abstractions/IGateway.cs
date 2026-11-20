using ThabeSoft.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Transports;

namespace ThabeSoft.EdgeGateway;


public interface IGateway : ILifecycleWithState
{
    IReadOnlyDictionary<PortId, IPort> Ports { get; }

    ValueTask<IPort> AddPortAsync(PortDescriptor descriptor, CancellationToken cancellationToken = default);
    ValueTask<bool> RemovePortAsync(PortId id, CancellationToken cancellationToken = default);
}

public static class GatewayExtensions
{
    extension(IGateway gateway)
    {
        public ValueTask<IPort> AddPortAsync(PortId id, TransportDescriptor transportDescriptor, CancellationToken cancellationToken = default) =>
            gateway.AddPortAsync(new PortDescriptor() { Transport = transportDescriptor, Id = id });
        public ValueTask<IPort> AddPortAsync(TransportDescriptor transportDescriptor, CancellationToken cancellationToken = default) =>
            gateway.AddPortAsync(PortId.Create(), transportDescriptor);
    }
}