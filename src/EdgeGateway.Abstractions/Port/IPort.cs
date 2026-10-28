using EdgeGateway.Channel;
using EdgeGateway.Device;

namespace EdgeGateway.Port;


public interface IPort : IPortLifecycle, IDeviceManager, IAsyncDisposable
{
    PortId Id { get; }
    ConnectionType ConnectionType { get; }
}