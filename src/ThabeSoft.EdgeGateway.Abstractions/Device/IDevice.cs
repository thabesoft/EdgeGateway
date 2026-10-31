using EdgeGateway.Polling;
using EdgeGateway.Protocol;

namespace EdgeGateway.Device;


public interface IDevice : IDeviceLifecycle, IPollingManager, IAsyncDisposable
{
    DeviceId Id { get; }
    ProtocolType ProtocolType { get; }
}