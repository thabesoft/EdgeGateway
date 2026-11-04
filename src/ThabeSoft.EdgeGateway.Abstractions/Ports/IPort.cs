using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Devices;

namespace ThabeSoft.EdgeGateway.Ports;


public interface IPort : IPortLifecycle, IDeviceManager, IAsyncDisposable
{
    ConnectionEndpoint Endpoint { get; }
}