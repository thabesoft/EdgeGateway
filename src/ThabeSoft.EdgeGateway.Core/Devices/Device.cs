using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class Device(
        DeviceDescriptor descriptor,
        ILifecycleStateMachine lifecycleStateMachine,
        ILifecycleStateable portLifecycleState,
        SubscriptionCollection collection,
        ILogger<Device>? logger = null
    ) : IDevice, IAsyncDisposable
{
    private DisposeNotifier _disposeNotifier = new(logger);

    public DeviceId Id => descriptor.Id;
    public ProtocolType ProtocolType => descriptor.Protocol.Type;
    public IReadOnlyDictionary<SubscriptionId, ISubscription> Subscriptions => collection.Items;
    public LifecycleState State => lifecycleStateMachine.State;
    public IDisposeObservable Disposed => _disposeNotifier;


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        lifecycleStateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        lifecycleStateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        lifecycleStateMachine.DisposeAsync(OnDisposeAsync);


    public ValueTask<ISubscription<T>> AddSubscriptionAsync<T>(
            SubscriptionDescriptor<T> subscriptionDescriptor,
            CancellationToken cancellationToken) where T : notnull
    {
        return lifecycleStateMachine.ExecuteAsync(ct =>
            collection.AddAsync(subscriptionDescriptor, ct), cancellationToken);
    }
    public ValueTask<bool> RemoveSubscriptionAsync(
        SubscriptionId subscriptionId,
        CancellationToken cancellationToken = default)
    {
        return lifecycleStateMachine.ExecuteAsync(ct =>
            collection.RemoveAsync(subscriptionId, ct), cancellationToken);
    }



    private ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (portLifecycleState.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("设备启动失败, 端口未启动");

        return collection.StartAllAsync(cancellationToken);
    }
    private ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        return collection.StopAllAsync(cancellationToken);
    }
    private async ValueTask OnDisposeAsync()
    {
        await _disposeNotifier.RaiseAsync(this);
        await _disposeNotifier.TryDisposeAsync();
    }
}