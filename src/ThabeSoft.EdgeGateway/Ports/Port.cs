using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class Port(in PortArgs args) :
    IPort, IAsyncDisposable
{
    private readonly ILogger _logger = args.Logger;
    private readonly ILifecycleStateMachine _stateMachine = args.PortStateMachine;
    private readonly DeviceManager _deviceCollection = args.DeviceManager;
    private readonly ILifecycleStateable _gatewayLifecycleStateable = args.GatewayLifecycleState;
    private readonly ILifecycle _transportConnectionLifeCycle = args.TransportConnectionLifeCycle;


    public PortId Id { get; } = args.Port.Id;
    public ConnectionEndpoint Endpoint { get; } = args.Port.Transport.Endpoint;
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => _deviceCollection.Items;
    public LifecycleState State => _stateMachine.State;
    public Func<ValueTask>? DisposedCallback { get; init; }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);



    public async ValueTask<IDevice> AddDeviceAsync(DeviceDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        return await _stateMachine.ExecuteAsync(ct =>
            _deviceCollection.AddAsync(descriptor, ct), cancellationToken);
    }
    public ValueTask<bool> RemoveDeviceAsync(DeviceId deviceId, CancellationToken cancellationToken = default)
    {
        return _stateMachine.ExecuteAsync(ct =>
            _deviceCollection.RemoveAsync(deviceId, ct), cancellationToken);
    }


    private async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (_gatewayLifecycleStateable.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("端口启动失败, 网关未启动");

        await _transportConnectionLifeCycle.StartAsync(cancellationToken);
        await _deviceCollection.StartAllAsync(cancellationToken);
    }
    private async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        List<Exception> errors = [];

        try
        {
            await _deviceCollection.StopAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            errors.Add(ex);
            _logger.LogError(ex, "端口({Endpoint})设备批处理停止失败", Endpoint);
        }


        try { await _transportConnectionLifeCycle.StopAsync(cancellationToken); }
        catch (Exception ex)
        {
            errors.Add(ex);
            _logger.LogError(ex, "端口传输器 {Endpoint} 停止失败", Endpoint);
        }

        if (errors.Count == 0) return;
        throw new AggregateException($"端口 {Endpoint} 停止失败", errors);

    }
    private async ValueTask OnDisposeAsync()
    {
        if (DisposedCallback is null) return;

        try { await DisposedCallback.Invoke(); }
        catch (Exception ex) { _logger.LogError(ex, "设备释放回调异常"); }
    }
}

internal readonly struct PortArgs
{
    // Gateway
    public required ILifecycleStateable GatewayLifecycleState { get; init => field = value.Required(); }

    // Port
    public required ILogger Logger { get; init => field = value.Required(); }
    public required PortDescriptor Port { get; init => field = value.Required(); }
    public required ILifecycle TransportConnectionLifeCycle { get; init => field = value.Required(); }
    public required ILifecycleStateMachine PortStateMachine { get; init => field = value.Required(); }


    // Device
    public required DeviceManager DeviceManager { get; init => field = value.Required(); }
}