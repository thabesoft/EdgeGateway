using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.App.Designs;

public sealed class DesignPort(IPortDescriptor portDescriptor) : IPort
{
    private readonly Dictionary<DeviceId, IDevice> _devices = [];


    public ChannelAddress ChannelAddress => portDescriptor.ChannelDescriptor.Address;
    public bool IsOpen { get; private set; }
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => _devices.AsReadOnly();


    public ValueTask OpenAsync(CancellationToken cancellationToken = default)
    {
        IsOpen = true;
        return ValueTask.CompletedTask;
    }
    public ValueTask CloseAsync(CancellationToken cancellationToken = default)
    {
        IsOpen = false;
        return ValueTask.CompletedTask;
    }

    public IDevice AddDevice(IDeviceDescriptor descriptor)
    {
        var device = new DesignDevice(descriptor);
        _devices[device.Id] = device;
        return device;
    }
    public ValueTask RemoveDeviceAsync(DeviceId id, CancellationToken cancellationToken = default)
    {
        _devices.Remove(id);
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _devices.Clear();
        return ValueTask.CompletedTask;
    }
}
