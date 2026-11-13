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
    private readonly DeviceSubscriptionFactory _subscriptionFactory;
    private readonly ILifecycleStateable _portLifecycleState;
    private readonly IProtocolAccessor _protocolAccessor;
    private readonly RecoveryPolicyHandler _recoveryPolicyHandler;
    private readonly ILifecycleKeyedCollection<SubscriptionId, IAsyncDisposableSubscription> _subscriptions;
    private readonly ILifecycleBatchExecutor _subscriptionsBatchExecutor;
    private readonly ILifecycleStateMachine _stateMachine;


    public Device(
            DeviceDescriptor deviceDescriptor,
            DeviceSubscriptionFactory subscriptionFactory,
            ILifecycleStateable portLifecycleState,
            IProtocolAccessor protocolAccessor,
            RecoveryPolicyHandler recoveryPolicyHandler,
            ILifecycleStateMachine stateMachine,
            ILogger<Device>? logger = null)
    {
        _deviceDescriptor = deviceDescriptor;
        _subscriptionFactory = subscriptionFactory;
        _portLifecycleState = portLifecycleState;
        _protocolAccessor = protocolAccessor;
        _recoveryPolicyHandler = recoveryPolicyHandler;

        _subscriptions = Lifecycle.KeyedCollection<SubscriptionId, IAsyncDisposableSubscription>(this, logger);
        Subscriptions = new ReadOnlyDictionaryView<SubscriptionId, IAsyncDisposableSubscription, ISubscription>(_subscriptions.Items);

        _subscriptionsBatchExecutor = Lifecycle.BatchExecutor(logger);
        _stateMachine = stateMachine;//Lifecycle.RetryableStateMachine(logger);
    }

    public DeviceId Id => _deviceDescriptor.Id;
    public ProtocolType ProtocolType => _deviceDescriptor.Protocol.Type;
    public IReadOnlyDictionary<SubscriptionId, ISubscription> Subscriptions { get; }
    public LifecycleState State => _stateMachine.State;



    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);


    public async ValueTask<ISubscription<T>> AddSubscriptionAsync<T>(
            SubscriptionDescriptor<T> subscriptionDescriptor,
            CancellationToken cancellationToken)
        where T : notnull
    {
        var item = await _stateMachine.ExecuteAsync(ct =>
            _subscriptions.AddAsync(
                subscriptionDescriptor.Id,
                () => _subscriptionFactory.Create(subscriptionDescriptor, new(subscriptionDescriptor, _protocolAccessor, _recoveryPolicyHandler)),
                ct),
           cancellationToken);

        return (Subscription<T>)item;
    }
    public ValueTask<bool> RemoveSubscriptionAsync(
        SubscriptionId subscriptionId,
        CancellationToken cancellationToken = default)
    {
        return _stateMachine.ExecuteAsync(ct =>
            _subscriptions.RemoveAsync(subscriptionId, ct),
            cancellationToken);
    }



    private ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (_portLifecycleState.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("设备启动失败, 端口未启动");

        return _subscriptionsBatchExecutor.StartAsync(_subscriptions.Values, cancellationToken);
    }
    private ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        return _subscriptionsBatchExecutor.StopAsync(_subscriptions.Values, cancellationToken);
    }
    private ValueTask OnDisposeAsync()
    {
        return _subscriptionsBatchExecutor.DisposeAsync(_subscriptions.Values);
    }
}