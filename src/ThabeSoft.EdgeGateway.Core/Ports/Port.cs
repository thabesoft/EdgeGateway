using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Devices;
using System.Collections.Concurrent;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class Port : IPort
{
    private readonly IChannel _channel;
    private readonly IDeviceFactory[] _deviceFactories;
    private readonly ConcurrentDictionary<DeviceId, IDevice> _devices = [];


    public bool IsOpen { get; private set; }
    public ChannelAddress ChannelAddress { get; }
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => _devices.AsReadOnly();



    private Port(IChannel channel, IEnumerable<IDeviceFactory> deviceFactories)
    {
        _channel = channel;
        _deviceFactories = [.. deviceFactories];
        
        ChannelAddress = _channel.Address;
    }


    public async ValueTask OpenAsync(CancellationToken cancellationToken = default)
    {
        if (IsOpen) return;
        IsOpen = true;

        await _channel.OpenAsync(cancellationToken);

        foreach (var i in _devices.Values)
            await i.EnableAsync(cancellationToken);
    }

    public async ValueTask CloseAsync(CancellationToken cancellationToken = default)
    {
        if (!IsOpen) return;
        IsOpen = false;


        foreach (var i in _devices.Values)
            await i.DisableAsync(cancellationToken);

        await _channel.CloseAsync(cancellationToken);
    }

    public IDevice AddDevice(IDeviceDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        var factory = _deviceFactories.FirstOrDefault(x => x.CanCreate(descriptor)) ??
            throw new InvalidOperationException();

        var device = factory.Create(descriptor, _channel);
        return _devices[device.Id] = device;
    }
    public async ValueTask RemoveDeviceAsync(DeviceId id, CancellationToken cancellationToken)
    {
        if (!_devices.TryRemove(id, out var device))
            return;

        await device.DisableAsync(cancellationToken);
    }



    public static Port Create(IChannelFactory channelFactory, IChannelDescriptor channelDescriptor, IEnumerable<IDeviceFactory> deviceFactories)
    {
        var channel = channelFactory.Create(channelDescriptor);
        return new(channel, deviceFactories);
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DisposeAsync();
        _devices.Clear();
    }
}