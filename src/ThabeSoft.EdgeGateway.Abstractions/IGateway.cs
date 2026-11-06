using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;


public interface IGateway : ILifecycle, IAsyncDisposable
{
    IReadOnlyDictionary<ConnectionEndpoint, IPort> Ports { get; }
    IPort AddPort(ITransportDescriptor transportDescriptor);
}