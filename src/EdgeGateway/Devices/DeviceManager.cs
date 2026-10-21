namespace EdgeGateway.Devices;


internal sealed class DeviceManager : IDeviceManager
{
    private readonly Dictionary<DeviceId, IDevice> _devices = [];
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => _devices;


    public IDevice Add(DeviceDescriptor descriptor)
    {
        var id = DeviceId.Create();
        return _devices[id] = new Device(this, id, descriptor);
    }

    public async ValueTask RemoveAsync(DeviceId id, CancellationToken cancellationToken = default)
    {
        if (!_devices.TryGetValue(id, out var device))
            return;

        await device.DisableAsync(cancellationToken);
        _devices.Remove(device.Id);
    }
}


file sealed class Device(IDeviceManager deviceManager, DeviceId deviceId, DeviceDescriptor descriptor) : IDevice
{
    public DeviceId Id { get; } = deviceId;
    public DeviceDescriptor Descriptor => descriptor;
    public bool IsEnable { get; private set; }


    public ValueTask DisableAsync(CancellationToken cancellationToken = default)
    {
        IsEnable = false;
        return ValueTask.CompletedTask;
    }

    public ValueTask EnableAsync(CancellationToken cancellationToken = default)
    {
        IsEnable = true;
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await deviceManager.RemoveAsync(Id);
    }
}