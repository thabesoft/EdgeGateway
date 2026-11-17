using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway;


internal sealed class Gateway(GatewayDependencies dependencies) :
    IGateway, IAsyncDisposable
{
    private readonly ILifecycleStateMachine _stateMachine = dependencies.GatwayStateMachine;
    private readonly PortManager _portManager = dependencies.PortManager;
    private readonly ILogger _logger = dependencies.Logger;


    public LifecycleState State => _stateMachine.State;
    public IReadOnlyDictionary<PortId, IPort> Ports => _portManager.Items;
    public Func<ValueTask>? DisposedCallback { get; init; }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(_portManager.StartAllAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(_portManager.StopAllAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);



    public async ValueTask<IPort> AddPortAsync(PortDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        return await _stateMachine.ExecuteAsync(ct =>
            _portManager.AddAsync(descriptor, ct), cancellationToken);
    }
    public async ValueTask<bool> RemovePortAsync(PortId id, CancellationToken cancellationToken = default)
    {
        return await _stateMachine.ExecuteAsync(ct =>
            _portManager.RemoveAsync(id, ct), cancellationToken);
    }

    private async ValueTask OnDisposeAsync()
    {
        if (DisposedCallback is null) return;

        try { await DisposedCallback.Invoke(); }
        catch (Exception ex) { _logger.LogError(ex, "网关释放回调执行异常"); }
    }
}

internal readonly struct GatewayDependencies
{
    // Gateway
    public required ILogger Logger { get; init => field = value.Required(); }
    public required ILifecycleStateMachine GatwayStateMachine { get; init => field = value.Required(); }

    // Port
    public required PortManager PortManager { get; init => field = value.Required(); }
}


internal sealed class GatewayFactory(
        ILifecycleStateMachineFactory lifecycleStateMachineFactory,
        IEventPublisher eventPublisher,
        PortManagerFactory portManagerFactory,
        PortFactory portFactory,
        ILoggerFactory loggerFactory
    )
{
    public Gateway Create(GatewayCreationContext context)
    {
        var logger = loggerFactory.CreateLogger<Gateway>();

        // 状态机
        var gatewayStateMachine = lifecycleStateMachineFactory.Create(context.GatewayStateMachine);
        var gatewayStateEventDisposable = gatewayStateMachine.StateChanged.Subscribe(x =>
            _ = eventPublisher.PublishAsync(new GatewayStateChangedEvent(x.Old, x.New)));


        var portManager = portManagerFactory.Create(new()
        {
            GatwayLifecycleState = gatewayStateMachine,
            PortBatchExecutorDescriptor = context.PortBatchExecutor,
            PortCreator = port => portFactory.Create(new()
            {
                GatewayLifecycleState = gatewayStateMachine,

                Port = port,
                PortStateMachine = context.PortStateMachine,
                TransportStateMachine = context.TransportStateMachine,

                DeviceStateMachine = context.DeviceStateMachine,
                DeviceBatchExecutor = context.DeviceBatchExecutorDescriptor,

                SubscriptionStateMachine = context.SubscriptionStateMachine,
                SubscriptionBatchExecutor = context.SubscriptionBatchExecutorDescriptor,
            })
        });

        // 网关依赖
        var gatewaytDependencies = new GatewayDependencies()
        {
            GatwayStateMachine = gatewayStateMachine,
            PortManager = portManager,
            Logger = logger
        };


        return new Gateway(gatewaytDependencies)
        {
            DisposedCallback = async () =>
            {
                await portManager.TryDisposeAsync();
                gatewayStateEventDisposable.TryDispose();
                await gatewayStateMachine.TryDisposeAsync();
            }
        };
    }
}


public sealed record GatewayCreationContext
{
    // Gateway
    public required LifecycleStateMachineDescriptor GatewayStateMachine { get; init => field = value.Required(); }

    // Port
    public required LifecycleStateMachineDescriptor PortStateMachine { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor TransportStateMachine { get; init => field = value.Required(); }
    public LifecycleBatchExecutorDescriptor PortBatchExecutor { get; init => field = value.Required(); } = new LifecycleBatchExecutorDescriptor();


    // Device
    public required LifecycleStateMachineDescriptor DeviceStateMachine { get; init => field = value.Required(); }
    public LifecycleBatchExecutorDescriptor DeviceBatchExecutorDescriptor { get; init => field = value.Required(); } = new LifecycleBatchExecutorDescriptor();


    // Subscription
    public required LifecycleStateMachineDescriptor SubscriptionStateMachine { get; init => field = value.Required(); }
    public LifecycleBatchExecutorDescriptor SubscriptionBatchExecutorDescriptor { get; init => field = value.Required(); } = new LifecycleBatchExecutorDescriptor();
}