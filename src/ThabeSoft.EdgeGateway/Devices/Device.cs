using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class Device(DeviceDependencies dependencies) :
    IDevice, IAsyncDisposable
{
    private readonly ILifecycleStateMachine _stateMachine = dependencies.DeviceStateMachine;
    private readonly SubscriptionManager _subscriptionManager = dependencies.SubscriptionManager;
    private readonly ILifecycleStateable _portState = dependencies.PortState;


    public DeviceId Id => dependencies.Device.Id;
    public ProtocolType ProtocolType => dependencies.Device.Protocol.Type;
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
        catch (Exception ex) { dependencies.Logger.LogError(ex, "设备释放回调异常"); }
    }
}

internal readonly struct DeviceDependencies
{
    // Port
    public required ILifecycleStateable PortState { get; init => field = value.Required(); }

    // Device
    public required ILogger Logger { get; init => field = value.Required(); }
    public required DeviceDescriptor Device { get; init => field = value.Required(); }
    public required ILifecycleStateMachine DeviceStateMachine { get; init => field = value.Required(); }

    // Subscription
    public required SubscriptionManager SubscriptionManager { get; init => field = value.Required(); }
}



internal sealed class DeviceFactory(
    ILifecycleStateMachineFactory lifecycleStateMachineFactory,
    SubscriptionManagerFactory subscriptionManagerFactory,
    IEventPublisher eventPublisher,
    SubscriptionFactory subscriptionFactory,
    ILoggerFactory loggerFactory
    )
{
    public Device Create(DeviceCreationContext context)
    {
        var logger = loggerFactory.CreateLogger<Device>();

        // 状态机
        var deviceStateMachine = lifecycleStateMachineFactory.Create(context.DeviceStateMachineDescriptor);
        var deviceStateEventDisposable = deviceStateMachine.StateChanged.Subscribe(x =>
            _ = eventPublisher.PublishAsync(new DeviceStateChangedEvent(context.Port.Id, context.Device.Id, x.Old, x.New)));

        // 订阅创建器
        var subscriptionCreator = new SubscriptionCreator(new()
        {
            Port = context.Port,
            ProtocolAccessor = context.ProtocolAccessor,

            Device = context.Device,
            DeviceState = deviceStateMachine,

            SubscriptionStateMachineDescriptor = context.SubscriptionStateMachineDescriptor,
            SubscriptionFactory = subscriptionFactory,
        });

        // 订阅管理器
        var subscriptionManager = subscriptionManagerFactory.Create(new()
        {
            DeviceId = context.Device.Id,
            PortId = context.Port.Id,
            DeviceState = deviceStateMachine,
            SubscriptionCreator = subscriptionCreator,
            SubscriptionBatchExecutorDescriptor = context.SubscriptionBatchExecutorDescriptor
        });

        // 订阅依赖
        var devicDependencies = new DeviceDependencies()
        {
            PortState = context.PortState,
            Logger = logger,
            Device = context.Device,
            DeviceStateMachine = deviceStateMachine,
            SubscriptionManager = subscriptionManager,
        };

        // 构建
        return new(devicDependencies)
        {
            DisposedCallback = async () =>
            {
                await subscriptionManager.TryDisposeAsync(logger);
                deviceStateEventDisposable.TryDispose(logger);
                await deviceStateMachine.TryDisposeAsync(logger);
            }
        };
    }


    private sealed class SubscriptionCreator(SubscriptionCreatorDependencies dependencies) : ISubscriptionCreator
    {
        public Subscription<T> Create<T>(SubscriptionDescriptor<T> descriptor) where T : notnull
        {
            return dependencies.SubscriptionFactory.Create<T>(new()
            {
                Port = dependencies.Port,
                ProtocolAccessor = dependencies.ProtocolAccessor,
                Device = dependencies.Device,
                DeviceState = dependencies.DeviceState,
                Subscription = descriptor,
                SubscriptioStateMachineDescriptor = dependencies.SubscriptionStateMachineDescriptor
            });
        }
    }

    private readonly struct SubscriptionCreatorDependencies
    {
        // Port
        public required PortDescriptor Port { get; init => field = value.Required(); }
        public required IProtocolAccessor ProtocolAccessor { get; init => field = value.Required(); }

        // Device
        public required DeviceDescriptor Device { get; init => field = value.Required(); }
        public required ILifecycleStateable DeviceState { get; init => field = value.Required(); }

        // Subscription
        public required LifecycleStateMachineDescriptor SubscriptionStateMachineDescriptor { get; init => field = value.Required(); }
        public required SubscriptionFactory SubscriptionFactory { get; init => field = value.Required(); }
    }
}


internal readonly struct DeviceCreationContext
{
    // Port
    public required PortDescriptor Port { get; init => field = value.Required(); }
    public required ILifecycleStateable PortState { get; init; }
    public required IProtocolAccessor ProtocolAccessor { get; init => field = value.Required(); }

    // Device
    public required DeviceDescriptor Device { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor DeviceStateMachineDescriptor { get; init; }


    // Subscription
    public required LifecycleStateMachineDescriptor SubscriptionStateMachineDescriptor { get; init => field = value.Required(); }
    public required LifecycleBatchExecutorDescriptor SubscriptionBatchExecutorDescriptor { get; init; }
}