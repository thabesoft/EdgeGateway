using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class Port(
        PortDescriptor descriptor,
        ILifecycleStateMachine lifecycleStateMachine,
        ILifecycleStateable gatewayLifecycleState,
        DeviceCollection deviceCollection,
        ILifecycle transportConnectionLifeCycle,
        ILogger<Port>? logger = null
    ) : IPort, IAsyncDisposable
{
    private DisposeNotifier _disposeNotifier = new(logger);
    private readonly ILogger<Port> _logger = logger ?? NullLogger<Port>.Instance;


    public IDisposeObservable Disposed => _disposeNotifier;
    public ConnectionEndpoint Endpoint => descriptor.Transport.Endpoint;
    public LifecycleState State => lifecycleStateMachine.State;
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => deviceCollection.Items;


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        lifecycleStateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        lifecycleStateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        lifecycleStateMachine.DisposeAsync(OnDisposeAsync);



    public async ValueTask<IDevice> AddDeviceAsync(DeviceDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        return await lifecycleStateMachine.ExecuteAsync(ct =>
            deviceCollection.AddAsync(descriptor, ct), cancellationToken);
    }
    public ValueTask<bool> RemoveDeviceAsync(DeviceId deviceId, CancellationToken cancellationToken = default)
    {
        return lifecycleStateMachine.ExecuteAsync(ct =>
            deviceCollection.RemoveAsync(deviceId, ct), cancellationToken);
    }


    private async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (gatewayLifecycleState.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("端口启动失败, 网关未启动");

        await transportConnectionLifeCycle.StartAsync(cancellationToken);
        await deviceCollection.StartAllAsync(cancellationToken);
    }
    private async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        List<Exception> errors = [];

        try
        {
            await deviceCollection.StopAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            errors.Add(ex);
            _logger.LogError(ex, "端口({Endpoint})设备批处理停止失败", Endpoint);
        }


        try { await transportConnectionLifeCycle.StopAsync(cancellationToken); }
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