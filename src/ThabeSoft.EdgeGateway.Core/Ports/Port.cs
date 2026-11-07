using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class Port(
        Func<ITransportConnection> transportConnectionFactory,
        Func<DeviceDescriptor, ITransportConnection, IDevice> deviceFactory,
        ILogger<Port> logger
    ) : RetryableLifecycle, IPort
{
    private ITransportConnection _transportConnection = transportConnectionFactory();
    private readonly ConcurrentDictionary<DeviceId, IDevice> _devices = [];


    public ConnectionEndpoint Endpoint => _transportConnection.Endpoint;
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => _devices;


    public IDevice AddDevice(DeviceDescriptor descriptor)
    {
        if (_devices.ContainsKey(descriptor.Id))
            throw new InvalidOperationException($"已存在相同 Id({descriptor.Id}) 的设备");

        var device = deviceFactory(descriptor, _transportConnection);
        _devices[device.Id] = device;

        LogAddDevice(device.Id);
        return device;
    }


    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        await _transportConnection.StartAsync(cancellationToken);

        foreach (var i in Devices.Values)
            await i.StartAsync(cancellationToken);

        LogStarted();
    }
    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        foreach (var i in Devices.Values)
            await i.StopAsync(cancellationToken);

        await _transportConnection.StopAsync(cancellationToken);
        LogStoped();
    }
    protected override async ValueTask OnDisposeAsync()
    {
        foreach (var i in _devices.Values)
            await i.DisposeAsync();

        await _transportConnection.DisposeAsync();
        LogDisposed();
    }


    private void LogStarted()
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;
        logger.LogDebug("端口 [{Endpoint}] 启动", Endpoint);
    }
    private void LogStoped()
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;
        logger.LogDebug("端口 [{Endpoint}] 停止", Endpoint);
    }
    private void LogAddDevice(DeviceId deviceId)
    {
        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("端口 [{Endpoint}] 添加设备 [{DeviceId}]", Endpoint, deviceId);
    }
    private void LogDisposed()
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;
        logger.LogDebug("端口 [{Endpoint}] 释放", Endpoint);
    }
}