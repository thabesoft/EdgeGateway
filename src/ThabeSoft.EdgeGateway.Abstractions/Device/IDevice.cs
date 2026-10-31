using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Protocol;

namespace ThabeSoft.EdgeGateway.Device;


public interface IDevice : IDeviceLifecycle, IPollingManager, IAsyncDisposable
{
    DeviceId Id { get; }
    ProtocolType ProtocolType { get; }
}