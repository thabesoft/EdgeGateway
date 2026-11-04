using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway;


internal sealed class Gateway(
        PortFactory portFactory,
        ILogger<Gateway> logger
    ) : ResilientLifecycleBase, IGateway
{
    private readonly ConcurrentDictionary<ConnectionEndpoint, IPort> _ports = [];
    public IReadOnlyDictionary<ConnectionEndpoint, IPort> Ports => _ports;


    public IPort AddPort(ITransportDescriptor transportDescriptor)
    {
        ArgumentNullException.ThrowIfNull(transportDescriptor);

        var port = portFactory.Create(transportDescriptor, this);
        return _ports[port.Endpoint] = port;
    }
    public async ValueTask RemovePortAsync(ConnectionEndpoint endpoint, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(endpoint);

        if (!_ports.TryRemove(endpoint, out var port)) return;
        await port.CloseAsync(cancellationToken);
    }
    public override string ToString()
    {
        return $"端口数量: {Ports.Count}";
    }


    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        List<Exception> exceptions = [];

        foreach (var port in _ports.Values)
        {
            try
            {
                await port.OpenAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count == 0) return;
        throw new AggregateException("部分端口开启失败", exceptions);
    }
    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        List<Exception> exceptions = [];

        foreach (var port in _ports.Values)
        {
            try
            {
                await port.CloseAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count == 0) return;
        throw new AggregateException("部分端口关闭失败", exceptions);
    }
    protected override async ValueTask OnDisposeAsync()
    {
        foreach (var port in _ports.Values)
        {
            try
            {
                await port.DisposeAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "端口释放失败");
            }
        }
    }
}



internal sealed class PortFactory(TransportFactory transportFactory, DeviceFactory deviceFactory)
{
    public Port Create(ITransportDescriptor transportDescriptor, IPortManager portManager)
    {
        return new(CreateTransport, CreateDevice, endpoint => portManager.RemovePortAsync(endpoint, default));

        ITransportConnection CreateTransport() =>
            transportFactory.Create(transportDescriptor);

        Device CreateDevice(DeviceDescriptor deviceDescriptor, IByteDuplex byteDuplex, IDeviceManager deviceManager) =>
            deviceFactory.Create(deviceDescriptor, byteDuplex, deviceManager);
    }
}

internal sealed class TransportFactory(IEnumerable<ITransportFactory> factories)
{
    public ITransportConnection Create(ITransportDescriptor channelDescriptor)
    {
        return factories
            .FirstOrDefault(x => x.CanCreate(channelDescriptor))
            ?.Create(channelDescriptor)
            ?? throw new InvalidOperationException($"无法创建端口, 不支持的描述: {channelDescriptor}");
    }
}


internal sealed class DeviceFactory(TagAccessorFactory tagAccessorFactory, ILogger<Device> logger)
{
    public Device Create(DeviceDescriptor descriptor, IByteDuplex byteDuplex, IDeviceManager deviceManager)
    {
        return new(descriptor, CreateTagAccessor, id => deviceManager.RemoveDeviceAsync(id, default), logger);

        ITagAccessor CreateTagAccessor() =>
            tagAccessorFactory.Create(descriptor.Protocol, byteDuplex);
    }
}

internal sealed class TagAccessorFactory(IEnumerable<ITagAccessorFactory> factories)
{
    public ITagAccessor Create(IProtocolDescriptor protocolDescriptor, IByteDuplex byteDuplex)
    {
        return factories
            .FirstOrDefault(x => x.CanCreate(protocolDescriptor))
            ?.Create(protocolDescriptor, byteDuplex)
            ?? throw new InvalidOperationException($"无法创建标签访问器, 不支持的描述: {protocolDescriptor}");
    }
}