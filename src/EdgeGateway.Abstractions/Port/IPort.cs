using EdgeGateway.Device;

namespace EdgeGateway.Port;


public interface IPort : IAsyncDisposable
{
    PortId Id { get; }
    bool IsOpen { get; }


    ValueTask OpenAsync(CancellationToken cancellationToken = default);
    ValueTask CloseAsync(CancellationToken cancellationToken = default);


    IDevice AddDevice(IDeviceDescriptor descriptor);
    void RemoveDevice(DeviceId id);
}
