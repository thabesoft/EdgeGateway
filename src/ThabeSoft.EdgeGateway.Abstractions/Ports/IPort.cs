using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Devices;

namespace ThabeSoft.EdgeGateway.Ports;


public interface IPort : IPortLifecycle, IDeviceManager, IAsyncDisposable
{
    PortId Id { get; }
    ConnectionType ConnectionType { get; }
}