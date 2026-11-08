using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class Port(
        ITransportDescriptor transportDescriptor,
        TransportConnectionFactorySelector transportConnectionFactorySelector,
        DeviceFactory deviceFactory,
        ILifecycleStateable gatewayLifecycleState,
        ILogger<Port> logger
    ) : IPort
{
    private readonly LifecycleStateMachineA _stateMachine = Lifecycle.Normal(x => x.StartStop(60), logger: logger);
    private readonly ITransportConnection _transportConnection = transportConnectionFactorySelector.Create(transportDescriptor);
    private readonly ConcurrentDictionary<DeviceId, IDevice> _devices = [];


    public ConnectionEndpoint Endpoint => _transportConnection.Endpoint;
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => _devices;
    public LifecycleState State => _stateMachine.State;



    public IDevice AddDevice(DeviceDescriptor descriptor)
    {
        return _stateMachine.Execute(() =>
        {
            if (_devices.ContainsKey(descriptor.Id))
                throw new InvalidOperationException($"已存在相同 Id({descriptor.Id}) 的设备");

            var device = deviceFactory.Create(descriptor, this, _transportConnection);
            _devices[device.Id] = device;

            return device;
        });
    }

    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
     _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);




    private async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (gatewayLifecycleState.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("端口启动失败, 网关未启动");

        await _transportConnection.StartAsync(cancellationToken);

        List<Exception> errors = [];
        foreach (var device in Devices.Values)
        {
            try
            {
                await device.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                logger.LogError(ex, "设备 {DeviceId} 启动失败", device.Id);
            }
        }

        if (errors.Count == 0) return;
        throw new AggregateException($"端口 {Endpoint} 启动时 {errors.Count} 个设备启动失败", errors);
    }
    private async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        List<Exception> errors = [];

        foreach (var device in Devices.Values)
        {
            try
            {
                await device.StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                logger.LogError(ex, "设备 {DeviceId} 启动失败", device.Id);
            }
        }

        try { await _transportConnection.StopAsync(cancellationToken); }
        catch (Exception ex)
        {
            errors.Add(ex);
            logger.LogError(ex, "传输器 {Endpoint} 停止失败", _transportConnection.Endpoint);
        }

        if (errors.Count == 0) return;
        throw new AggregateException($"端口 {Endpoint} 停止时 {errors.Count} 个端口停止失败", errors);

    }
    private async ValueTask OnDisposeAsync()
    {
        foreach (var device in _devices.Values)
        {
            try { await device.DisposeAsync(); }
            catch (Exception ex)
            {
                logger.LogError(ex, "设备 {DeviceId} 释放失败", device.Id);
            }
        }

        try
        {
            await _transportConnection.DisposeAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "端口 {Endpoint} 传输器 释放失败", _transportConnection.Endpoint);
        }
    }
}