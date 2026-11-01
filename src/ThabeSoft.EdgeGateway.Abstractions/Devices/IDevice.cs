using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Devices;


public interface IDevice : IDeviceLifecycle, IPollingManager, IAsyncDisposable
{
    DeviceId Id { get; }
    ProtocolType ProtocolType { get; }
}