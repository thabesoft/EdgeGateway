namespace EdgeGateway.Device;

public interface IDeviceManager
{
    IReadOnlyDictionary<DeviceId, IDevice> Devices { get; }

    IDevice AddDevice(IDeviceDescriptor descriptor);
    void RemoveDevice(DeviceId id);
}
