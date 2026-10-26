using EdgeGateway.Port;

namespace EdgeGateway;


public interface IGateway : IAsyncDisposable
{
    ValueTask StartAsync(CancellationToken cancellationToken = default);
    ValueTask StopAsync(CancellationToken cancellationToken = default);


    IPort AddPort(IPortDescriptor descriptor);
    ValueTask RemovePortAsync(PortId id, CancellationToken cancellationToken = default);
}