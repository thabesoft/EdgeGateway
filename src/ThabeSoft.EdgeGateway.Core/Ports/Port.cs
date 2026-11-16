using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class Port(PortDependencies dependencies) :
    IPort, IAsyncDisposable
{
    private readonly DisposeNotifier _disposeNotifier = new(dependencies.Logger);
    private readonly ILogger<Port> _logger = dependencies.Logger;
    private readonly ILifecycleStateMachine _stateMachine = dependencies.LifecycleStateMachine;
    private readonly DeviceCollection _deviceCollection = dependencies.DeviceCollection;
    private readonly ILifecycleStateable _gatewayLifecycleStateable = dependencies.GatewayLifecycleState;
    private readonly ILifecycle _transportConnectionLifeCycle = dependencies.TransportConnectionLifeCycle;


    public PortId Id { get; } = dependencies.Descriptor.Id;
    public ConnectionEndpoint Endpoint { get; } = dependencies.Descriptor.Transport.Endpoint;
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => _deviceCollection.Items;
    public LifecycleState State => _stateMachine.State;
    public IDisposeObservable Disposed => _disposeNotifier;


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
        await _disposeNotifier.RaiseAsync(this);
        await _disposeNotifier.TryDisposeAsync(_logger);
    }
}

internal readonly struct PortDependencies
{
    public required PortDescriptor Descriptor { get; init => field = value.Required(); }
    public required ILifecycleStateable GatewayLifecycleState { get; init => field = value.Required(); }
    public required DeviceCollection DeviceCollection { get; init => field = value.Required(); }
    public required ILifecycle TransportConnectionLifeCycle { get; init => field = value.Required(); }

    public required ILifecycleStateMachine LifecycleStateMachine { get; init => field = value.Required(); } 
    public required ILogger<Port> Logger { get; init => field = value.Required(); }
}