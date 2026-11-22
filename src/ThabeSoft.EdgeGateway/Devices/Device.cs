using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Primitives;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class Device(DeviceDependencies dependencies) : IDevice, IAsyncDisposable
{
    private readonly DisposeFlag _disposeFlag = new();


    public DeviceId Id { get; } = dependencies.Descriptor.Id;
    public ProtocolType ProtocolType { get; } = dependencies.Descriptor.Protocol.Type;
    public IReadOnlyDictionary<SubscriptionId, ISubscription> Subscriptions => dependencies.SubscriptionManager.Items;
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


    public ValueTask<ISubscription<T>> AddSubscriptionAsync<T>(
        SubscriptionDescriptor<T> subscriptionDescriptor,
        CancellationToken cancellationToken) where T : notnull
    {
        return dependencies.StateMachine.ExecuteAsync(ct =>
            dependencies.SubscriptionManager.AddAsync(subscriptionDescriptor, ct), cancellationToken);
    }
    public ValueTask<bool> RemoveSubscriptionAsync(
        SubscriptionId subscriptionId,
        CancellationToken cancellationToken = default)
    {
        return dependencies.StateMachine.ExecuteAsync(ct =>
            dependencies.SubscriptionManager.RemoveAsync(subscriptionId, ct), cancellationToken);
    }


    private ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (dependencies.PortState.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("设备启动失败, 端口未启动");

        return dependencies.SubscriptionManager.StartAllAsync(cancellationToken);
    }
    private ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        return dependencies.SubscriptionManager.StopAllAsync(cancellationToken);
    }
}


internal sealed class DeviceDependencies
{
    // Port
    public required PortDescriptor Port { get; init => field = value.Required(); }
    public required ILifecycleStateable PortState { get; init => field = value.Required(); }

    // Device
    public required DeviceDescriptor Descriptor { get; init => field = value.Required(); }
    public required ILifecycleStateMachine StateMachine { get; init => field = value.Required(); }

    // Subscription
    public required SubscriptionManager SubscriptionManager { get; init => field = value.Required(); }


    // Services
    public required ILogger<Device> Logger { get; init => field = value.Required(); }
    public required Func<ValueTask> OnDispose { get; init => field = value.Required(); }
}