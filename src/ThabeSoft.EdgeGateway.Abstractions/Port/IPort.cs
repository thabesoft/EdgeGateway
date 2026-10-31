using ThabeSoft.EdgeGateway.Channel;
using ThabeSoft.EdgeGateway.Device;

namespace ThabeSoft.EdgeGateway.Port;


public interface IPort : IPortLifecycle, IDeviceManager, IAsyncDisposable
{
    PortId Id { get; }
    ConnectionType ConnectionType { get; }
}