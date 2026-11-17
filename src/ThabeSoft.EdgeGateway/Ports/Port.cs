using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class Port(PortDependencies dependencies) :
    IPort, IAsyncDisposable
{
    private readonly ILogger _logger = dependencies.Logger;
    private readonly ILifecycleStateMachine _stateMachine = dependencies.PortStateMachine;
    private readonly DeviceManager _deviceCollection = dependencies.DeviceManager;
    private readonly ILifecycleStateable _gatewayLifecycleStateable = dependencies.GatewayLifecycleState;
    private readonly ILifecycle _transportConnectionLifeCycle = dependencies.TransportConnectionLifeCycle;


    public PortId Id { get; } = dependencies.Port.Id;
    public ConnectionEndpoint Endpoint { get; } = dependencies.Port.Transport.Endpoint;
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => _deviceCollection.Items;
    public LifecycleState State => _stateMachine.State;
    public Func<ValueTask>? DisposedCallback { get; init; }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);



    public async ValueTask<IDevice> AddDeviceAsync(DeviceDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        return await _stateMachine.ExecuteAsync(ct =>
            _deviceCollection.AddAsync(descriptor, ct), cancellationToken);
    }
    public ValueTask<bool> RemoveDeviceAsync(DeviceId deviceId, CancellationToken cancellationToken = default)
    {
        return _stateMachine.ExecuteAsync(ct =>
            _deviceCollection.RemoveAsync(deviceId, ct), cancellationToken);
    }


    private async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (_gatewayLifecycleStateable.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("端口启动失败, 网关未启动");

        await _transportConnectionLifeCycle.StartAsync(cancellationToken);
        await _deviceCollection.StartAllAsync(cancellationToken);
    }
    private async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        List<Exception> errors = [];

        try
        {
            await _deviceCollection.StopAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            errors.Add(ex);
            _logger.LogError(ex, "端口({Endpoint})设备批处理停止失败", Endpoint);
        }


        try { await _transportConnectionLifeCycle.StopAsync(cancellationToken); }
        catch (Exception ex)
        {
            errors.Add(ex);
            _logger.LogError(ex, "端口传输器 {Endpoint} 停止失败", Endpoint);
        }

        if (errors.Count == 0) return;
        throw new AggregateException($"端口 {Endpoint} 停止失败", errors);

    }
    private async ValueTask OnDisposeAsync()
    {
        if (DisposedCallback is null) return;

        try { await DisposedCallback.Invoke(); }
        catch (Exception ex) { dependencies.Logger.LogError(ex, "设备释放回调异常"); }
    }
}

internal readonly struct PortDependencies
{
    // Gateway
    public required ILifecycleStateable GatewayLifecycleState { get; init => field = value.Required(); }

    // Port
    public required ILogger Logger { get; init => field = value.Required(); }
    public required PortDescriptor Port { get; init => field = value.Required(); }
    public required ILifecycle TransportConnectionLifeCycle { get; init => field = value.Required(); }
    public required ILifecycleStateMachine PortStateMachine { get; init => field = value.Required(); }


    // Device
    public required DeviceManager DeviceManager { get; init => field = value.Required(); }
}


internal sealed class PortFactory(
        ILifecycleStateMachineFactory lifecycleStateMachineFactory,
        IEventPublisher eventPublisher,
        DeviceManagerFactory deviceManagerFactory,
        ITransportConnectionFactory transportConnectionFactory,
        ProtocolAccessorFactory protocolAccessorResolver,
        DeviceFactory deviceFactory,
        ILoggerFactory loggerFactory
    )
{
    public Port Create(PortCreationContext context)
    {
        var logger = loggerFactory.CreateLogger<Port>();

        // 状态机
        var portStateMachine = lifecycleStateMachineFactory.Create(context.PortStateMachine);
        var deviceStateEventDisposable = portStateMachine.StateChanged.Subscribe(x =>
            _ = eventPublisher.PublishAsync(new PortStateChangedEvent(context.Port.Id, x.Old, x.New)));

        // 传输器
        var transport = transportConnectionFactory.Create(new()
        {
            Transport = context.Port.Transport,
            StateMachine = context.TransportStateMachine
        });
        // 协议访问器
        var protocolAccessors = new ConcurrentDictionary<ProtocolDescriptor, IProtocolAccessor>();

        // 设备管理器
        var deviceManager = deviceManagerFactory.Create(new()
        {
            Port = context.Port,
            PortLifecycleState = portStateMachine,

            DeviceBatchExecutorDescriptor = context.DeviceBatchExecutor,
            DevicCreator = device => deviceFactory.Create(new()
            {
                Port = context.Port,
                ProtocolAccessor = protocolAccessors.GetOrAdd(device.Protocol, protocol => protocolAccessorResolver.Create(protocol, transport)),
                PortState = portStateMachine,

                Device = device,
                DeviceStateMachineDescriptor = context.DeviceStateMachine,

                SubscriptionBatchExecutorDescriptor = context.SubscriptionBatchExecutor,
                SubscriptionStateMachineDescriptor = context.SubscriptionStateMachine
            })
        });

        // 端口依赖
        var portDependencies = new PortDependencies()
        {
            GatewayLifecycleState = context.GatewayLifecycleState,

            Port = context.Port,
            PortStateMachine = portStateMachine,

            DeviceManager = deviceManager,
            TransportConnectionLifeCycle = transport,

            Logger = logger
        };

        return new Port(portDependencies)
        {
            DisposedCallback = async () =>
            {
                await deviceManager.TryDisposeAsync();
                protocolAccessors.Clear();
                await transport.TryDisposeAsync();
                deviceStateEventDisposable.TryDispose();
                await portStateMachine.TryDisposeAsync();
            }
        };
    }
}


internal readonly struct PortCreationContext
{
    // Gateway
    public required ILifecycleStateable GatewayLifecycleState { get; init => field = value.Required(); }


    // Port
    public required PortDescriptor Port { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor PortStateMachine { get; init; }
    public required LifecycleStateMachineDescriptor TransportStateMachine { get; init => field = value.Required(); }

    // Device
    public required LifecycleStateMachineDescriptor DeviceStateMachine { get; init => field = value.Required(); }
    public required LifecycleBatchExecutorDescriptor DeviceBatchExecutor { get; init => field = value.Required(); }


    // Subscription
    public required LifecycleStateMachineDescriptor SubscriptionStateMachine { get; init => field = value.Required(); }
    public required LifecycleBatchExecutorDescriptor SubscriptionBatchExecutor { get; init => field = value.Required(); }
}