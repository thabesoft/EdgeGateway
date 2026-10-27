using EdgeGateway.Channel;
using EdgeGateway.Device;

namespace EdgeGateway.Port;


public interface IPort : IAsyncDisposable
{
    PortId Id { get; }
    bool IsOpen { get; }
    ConnectionType ConnectionType { get; }



    ValueTask OpenAsync(CancellationToken cancellationToken = default);
    ValueTask CloseAsync(CancellationToken cancellationToken = default);



    IReadOnlyDictionary<DeviceId, IDevice> Devices { get; }
    IDevice AddDevice(IDeviceDescriptor descriptor);
    void RemoveDevice(DeviceId id);
}
