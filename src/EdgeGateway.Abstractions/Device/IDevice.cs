using EdgeGateway.Protocol;

namespace EdgeGateway.Device;


public interface IDevice : IDeviceLifecycle, IAsyncDisposable
{
    DeviceId Id { get; }
    ProtocolType ProtocolType { get; }
}