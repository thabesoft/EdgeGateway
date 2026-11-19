using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Primitives;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class Device(
        DeviceId id,
        ProtocolType protocolType,
        ILifecycleStateMachine stateMachine,
        SubscriptionManager subscriptionManager,
        ILifecycleStateable portState,
        ILogger<Device> logger,
        Func<ValueTask> onDispose
    ) : IDevice, IAsyncDisposable
{
    private readonly DisposeFlag _disposeFlag = new();


    public DeviceId Id { get; } = id;
    public ProtocolType ProtocolType { get; } = protocolType;
    public IReadOnlyDictionary<SubscriptionId, ISubscription> Subscriptions => subscriptionManager.Items;
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


    public ValueTask<ISubscription<T>> AddSubscriptionAsync<T>(
        SubscriptionDescriptor<T> subscriptionDescriptor,
        CancellationToken cancellationToken) where T : notnull
    {
        return stateMachine.ExecuteAsync(ct =>
            subscriptionManager.AddAsync(subscriptionDescriptor, ct), cancellationToken);
    }
    public ValueTask<bool> RemoveSubscriptionAsync(
        SubscriptionId subscriptionId,
        CancellationToken cancellationToken = default)
    {
        return stateMachine.ExecuteAsync(ct =>
            subscriptionManager.RemoveAsync(subscriptionId, ct), cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposeFlag.TryMarkDisposed()) return;
        await Safe.TryEexecuteAsync(onDispose, logger);
    }



    private ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (portState.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("设备启动失败, 端口未启动");

        return subscriptionManager.StartAllAsync(cancellationToken);
    }
    private ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        return subscriptionManager.StopAllAsync(cancellationToken);
    }
}