using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class Device(in DeviceArgs dependencies, DeviceDescriptor descriptor) :
    IDevice, IAsyncDisposable
{
    private readonly ILifecycleStateMachine _stateMachine = dependencies.DeviceStateMachine;
    private readonly SubscriptionManager _subscriptionManager = dependencies.SubscriptionManager;
    private readonly ILifecycleStateable _portState = dependencies.PortState;
    private readonly ILogger _logger = dependencies.Logger;


    public DeviceId Id { get; } = descriptor.Id;
    public ProtocolType ProtocolType { get; } = descriptor.Protocol.Type;
    public IReadOnlyDictionary<SubscriptionId, ISubscription> Subscriptions => _subscriptionManager.Items;
    public LifecycleState State => _stateMachine.State;
    public Func<ValueTask>? DisposedCallback { get; init; }



    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);


    public ValueTask<ISubscription<T>> AddSubscriptionAsync<T>(
        SubscriptionDescriptor<T> subscriptionDescriptor,
        CancellationToken cancellationToken) where T : notnull
    {
        return _stateMachine.ExecuteAsync(ct =>
            _subscriptionManager.AddAsync(subscriptionDescriptor, ct), cancellationToken);
    }
    public ValueTask<bool> RemoveSubscriptionAsync(
        SubscriptionId subscriptionId,
        CancellationToken cancellationToken = default)
    {
        return _stateMachine.ExecuteAsync(ct =>
            _subscriptionManager.RemoveAsync(subscriptionId, ct), cancellationToken);
    }



    private ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (_portState.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("设备启动失败, 端口未启动");

        return _subscriptionManager.StartAllAsync(cancellationToken);
    }
    private ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        return _subscriptionManager.StopAllAsync(cancellationToken);
    }
    private async ValueTask OnDisposeAsync()
    {
        if (DisposedCallback is null) return;

        try { await DisposedCallback.Invoke(); }
        catch (Exception ex) { _logger.LogError(ex, "设备释放回调异常"); }
    }
}

internal readonly struct DeviceArgs
{
    // Port
    public required ILifecycleStateable PortState { get; init => field = value.Required(); }

    // Device
    public required ILogger Logger { get; init => field = value.Required(); }
    public required ILifecycleStateMachine DeviceStateMachine { get; init => field = value.Required(); }

    // Subscription
    public required SubscriptionManager SubscriptionManager { get; init => field = value.Required(); }
}