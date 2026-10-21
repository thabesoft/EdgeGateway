using EdgeGateway.Channels;
using EdgeGateway.Devices;
using System.Collections.Concurrent;

namespace EdgeGateway.Ports;


public interface IPort : IAsyncDisposable
{
    PortId Id { get; }
    bool IsOpen { get; }


    ValueTask OpenAsync(CancellationToken cancellationToken = default);
    ValueTask CloseAsync(CancellationToken cancellationToken = default);


    IDevice AddDevice(IDeviceDescriptor descriptor);
    void RemoveDevice(DeviceId id);
}


public sealed class Port : IPort
{
    private readonly IChannel _channel;
    private readonly IDeviceFactory[] _deviceFactories;
    private readonly ConcurrentDictionary<DeviceId, IDevice> _devices = [];


    public PortId Id { get; }
    public bool IsOpen { get; private set; }


    private Port(IChannel channel, IEnumerable<IDeviceFactory> deviceFactories)
    {
        _channel = channel;
        Id = PortId.Parse(channel.Address.ToString());
        _deviceFactories = [.. deviceFactories];
    }


    public async ValueTask OpenAsync(CancellationToken cancellationToken = default)
    {
        await _channel.OpenAsync(cancellationToken);
        IsOpen = true;
    }

    public async ValueTask CloseAsync(CancellationToken cancellationToken = default)
    {
        IsOpen = false;
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
    public void RemoveDevice(DeviceId id)
    {
        _devices.TryRemove(id, out var _);
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