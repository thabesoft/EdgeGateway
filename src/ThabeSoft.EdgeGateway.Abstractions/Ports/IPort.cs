using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.Ports;


public interface IPort : ILifecycle, IDeviceManager, IAsyncDisposable
{
    ConnectionEndpoint Endpoint { get; }
}