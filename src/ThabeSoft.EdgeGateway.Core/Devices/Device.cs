using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Recoveries;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class Device : IDevice, IAsyncDisposable
{
    private readonly DeviceDescriptor _deviceDescriptor;
    private readonly SubscriptionFactory _subscriptionFactory;
    private readonly ILifecycleStateable _portLifecycleState;
    private readonly IProtocolAccessor _protocolAccessor;
    private readonly RecoveryPolicyHandler _exceptionHandler;
    private readonly ILifecycleKeyedCollection<SubscriptionId, IAsyncDisposeSubscription> _subscriptions;
    private readonly ILifecycleBatchExecutor _subscriptionsBatchExecutor;
    private readonly IRetryableLifecycleStateMachine _stateMachine;


    public Device(
            DeviceDescriptor deviceDescriptor,
            SubscriptionFactory subscriptionFactory,
            ILifecycleStateable portLifecycleState,
            IProtocolAccessor protocolAccessor,
            RecoveryPolicyHandler recoveryPolicyHandler,
            ILogger<Device> logger
    )
    {
        _deviceDescriptor = deviceDescriptor;
        _subscriptionFactory = subscriptionFactory;
        _portLifecycleState = portLifecycleState;
        _protocolAccessor = protocolAccessor;
        _exceptionHandler = recoveryPolicyHandler;

        _subscriptions = Lifecycle.KeyedCollection<SubscriptionId, IAsyncDisposeSubscription>(this, logger);
        Subscriptions = new ReadOnlyDictionaryView<SubscriptionId, IAsyncDisposeSubscription, ISubscription>(_subscriptions.Items);

        _subscriptionsBatchExecutor = Lifecycle.BatchExecutor(logger);
        _stateMachine = Lifecycle.RetryableStateMachine(logger);
    }

    public DeviceId Id => _deviceDescriptor.Id;
    public ProtocolType ProtocolType => _deviceDescriptor.Protocol.Type;
    public IReadOnlyDictionary<SubscriptionId, ISubscription> Subscriptions { get; }
    public LifecycleState State => _stateMachine.State;



    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        return _stateMachine.StartAsync(ct =>
        {
            if (_portLifecycleState.State is not (LifecycleState.Starting or LifecycleState.Running))
                throw new InvalidOperationException("设备启动失败, 端口未启动");

            return _subscriptionsBatchExecutor.StartAsync(_subscriptions.Values, ct);

        }, cancellationToken);
    }
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(ct => _subscriptionsBatchExecutor.StopAsync(_subscriptions.Values, ct), cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(() => _subscriptionsBatchExecutor.DisposeAsync(_subscriptions.Values));


    public async ValueTask<ISubscription<T>> AddSubscriptionAsync<T>(
            SubscriptionDescriptor<T> subscriptionDescriptor,
            CancellationToken cancellationToken)
        where T : notnull
    {
        var item = await _stateMachine.ExecuteAsync(
           ct => _subscriptions.AddAsync(
               subscriptionDescriptor.Id,
               () => _subscriptionFactory.Create(subscriptionDescriptor, _protocolAccessor, this, _exceptionHandler),
               ct),
           cancellationToken);

        return (Subscription<T>)item;
    }
    public ValueTask<bool> RemoveSubscriptionAsync(
        SubscriptionId subscriptionId,
        CancellationToken cancellationToken = default)
    {
        return _stateMachine.ExecuteAsync(
          ct => _subscriptions.RemoveAsync(subscriptionId, ct),
          cancellationToken);
    }
}