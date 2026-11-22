using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Primitives;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.Transports;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class Port(PortDependencies dependencies) : IPort, IAsyncDisposable
{
    private readonly DisposeFlag _disposeFlag = new();


    public PortId Id { get; } = dependencies.Descriptor.Id;
    public ConnectionEndpoint Endpoint { get; } = dependencies.Descriptor.Transport.Endpoint;
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => dependencies.DeviceManager.Items;
    public LifecycleState State => dependencies.StateMachine.State;



    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);
        return dependencies.StateMachine.TransitionToRunningAsync(OnStartAsync, cancellationToken);
    }
    public ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);
        return dependencies.StateMachine.TransitionToStoppedAsync(OnStopAsync, cancellationToken);
    }
    public async ValueTask DisposeAsync()
    {
        if (!_disposeFlag.TryMarkDisposed()) return;
        await Safe.TryEexecuteAsync(dependencies.OnDispose, dependencies.Logger);
    }


    public async ValueTask<IDevice> AddDeviceAsync(DeviceDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);

        return await dependencies.StateMachine.ExecuteAsync(ct =>
            dependencies.DeviceManager.AddAsync(descriptor, ct), cancellationToken);
    }
    public ValueTask<bool> RemoveDeviceAsync(DeviceId deviceId, CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);

        return dependencies.StateMachine.ExecuteAsync(ct =>
            dependencies.DeviceManager.RemoveAsync(deviceId, ct), cancellationToken);
    }


    private async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (dependencies.GatewayState.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("端口启动失败, 网关未启动");

        await dependencies.TransportConnectionLifecycle.StartAsync(cancellationToken);
        await dependencies.DeviceManager.StartAllAsync(cancellationToken);
    }
    private async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        List<Exception> errors = [];

        try
        {
            await dependencies.DeviceManager.StopAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            errors.Add(ex);
            dependencies.Logger.LogError(ex, "端口({Endpoint})设备批处理停止失败", Endpoint);
        }


        try { await dependencies.TransportConnectionLifecycle.StopAsync(cancellationToken); }
        catch (Exception ex)
        {
            errors.Add(ex);
            dependencies.Logger.LogError(ex, "端口传输器 {Endpoint} 停止失败", Endpoint);
        }

        if (errors.Count == 0) return;
        throw new AggregateException($"端口 {Endpoint} 停止失败", errors);
    }
}

internal sealed class PortDependencies
{
    // Gateway
    public required ILifecycleStateable GatewayState { get; init => field = value.Required(); }

    // Port
    public required PortDescriptor Descriptor { get; init => field = value.Required(); }
    public required ILifecycleStateMachine StateMachine { get; init => field = value.Required(); }
    public required ILifecycle TransportConnectionLifecycle { get; init => field = value.Required(); }

    // Device
    public required DeviceManager DeviceManager { get; init => field = value.Required(); }


    // Services
    public required Func<ValueTask> OnDispose { get; init => field = value.Required(); }
    public required ILogger<Port> Logger { get; init => field = value.Required(); }
}