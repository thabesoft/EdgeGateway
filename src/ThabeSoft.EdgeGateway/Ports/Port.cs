using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.Primitives;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class Port(
        PortId portId,
        ConnectionEndpoint connectionEndpoint,
        ILifecycleStateable gatewayLifecycleStateable,
        ILifecycleStateMachine stateMachine,
        DeviceManager deviceManager,
        ILifecycle transportConnectionLifeCycle,
        ILogger<Port> logger,
        Func<ValueTask> onDispose
    ) :
    IPort, IAsyncDisposable
{
    private readonly DisposeFlag _disposeFlag = new();


    public PortId Id { get; } = portId;
    public ConnectionEndpoint Endpoint { get; } = connectionEndpoint;
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => deviceManager.Items;
    public LifecycleState State => stateMachine.State;



    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);
        return stateMachine.TransitionToRunningAsync(OnStartAsync, cancellationToken);
    }
    public ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);
        return stateMachine.TransitionToStoppedAsync(OnStopAsync, cancellationToken);
    }


    public async ValueTask<IDevice> AddDeviceAsync(DeviceDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);

        return await stateMachine.ExecuteAsync(ct =>
            deviceManager.AddAsync(descriptor, ct), cancellationToken);
    }
    public ValueTask<bool> RemoveDeviceAsync(DeviceId deviceId, CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);

        return stateMachine.ExecuteAsync(ct =>
            deviceManager.RemoveAsync(deviceId, ct), cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposeFlag.TryMarkDisposed()) return;
        await Safe.TryEexecuteAsync(onDispose, logger);
    }



    private async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (gatewayLifecycleStateable.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("端口启动失败, 网关未启动");

        await transportConnectionLifeCycle.StartAsync(cancellationToken);
        await deviceManager.StartAllAsync(cancellationToken);
    }
    private async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        List<Exception> errors = [];

        try
        {
            await deviceManager.StopAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            errors.Add(ex);
            logger.LogError(ex, "端口({Endpoint})设备批处理停止失败", Endpoint);
        }


        try { await transportConnectionLifeCycle.StopAsync(cancellationToken); }
        catch (Exception ex)
        {
            errors.Add(ex);
            logger.LogError(ex, "端口传输器 {Endpoint} 停止失败", Endpoint);
        }

        if (errors.Count == 0) return;
        throw new AggregateException($"端口 {Endpoint} 停止失败", errors);
    }
}