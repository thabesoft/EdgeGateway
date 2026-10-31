namespace ThabeSoft.EdgeGateway.Device;

public interface IDeviceManager
{
    IReadOnlyDictionary<DeviceId, IDevice> Devices { get; }

    IDevice AddDevice(IDeviceDescriptor descriptor);
    ValueTask RemoveDeviceAsync(DeviceId id, CancellationToken cancellationToken = default);
}
