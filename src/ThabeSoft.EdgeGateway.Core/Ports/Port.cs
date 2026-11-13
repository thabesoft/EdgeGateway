using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Recoveries;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class Port : IPort, IAsyncDisposable
{
    // 设备资源
    private readonly DeviceFactory _deviceFactory;
    private readonly ILifecycleKeyedCollection<DeviceId, Device> _devices;
    private readonly ILifecycleBatchExecutor _deviceBatchExecutor;


    // 传输资源
    private readonly RecoveryPolicyHandler _recoveryPolicyHandler;
    private readonly ITransportConnection _transportConnection;
    private readonly ProtocolAccessorFactoryResolver _protocolAccessorFactoryResolver;
    private readonly ConcurrentDictionary<IProtocolDescriptor, IProtocolAccessor> _protocolAccessors = [];


    // 生命周期
    private readonly IRetryableLifecycleStateMachine _lifecycleStateMachine;
    private readonly PortDescriptor _portDescriptor;
    private readonly ILifecycleStateable _gatewayLifecycleState;


    // 日志
    private readonly ILogger<Port> _logger;

    public Port(
        PortDescriptor portDescriptor,
        ILifecycleStateable gatewayLifecycleState,
        TransportConnectionFactoryResolver transportConnectionFactoryResolver,
        ProtocolAccessorFactoryResolver protocolAccessorFactoryResolver,
        RecoveryPolicyFactoryResolver recoveryPolicyFactoryResolver,
        DeviceFactory deviceFactory,
        ILogger<Port> logger)
    {
        _portDescriptor = portDescriptor;
        _gatewayLifecycleState = gatewayLifecycleState;
        _deviceFactory = deviceFactory;
        _logger = logger;


        // 重试策略
        _recoveryPolicyHandler = new RecoveryPolicyHandler(_portDescriptor.Recoveries
            .Select(recoveryPolicyFactoryResolver.Resolve));

        // 协议/传输
        _transportConnection = transportConnectionFactoryResolver.Resolve(portDescriptor.Transport);
        _protocolAccessorFactoryResolver = protocolAccessorFactoryResolver;

        // 设备
        _devices = Lifecycle.KeyedCollection<DeviceId, Device>(this, logger);
        Devices = new ReadOnlyDictionaryView<DeviceId, Device, IDevice>(_devices.Items);
        _deviceBatchExecutor = Lifecycle.BatchExecutor(logger);

        // 状态机
        _lifecycleStateMachine = Lifecycle.RetryableStateMachine(logger);
    }

    public ConnectionEndpoint Endpoint => _transportConnection.Endpoint;
    public IReadOnlyDictionary<DeviceId, IDevice> Devices { get; }
    public LifecycleState State => _lifecycleStateMachine.State;


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _lifecycleStateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _lifecycleStateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _lifecycleStateMachine.DisposeAsync(OnDisposeAsync);



    public async ValueTask<IDevice> AddDeviceAsync(DeviceDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        return await _lifecycleStateMachine.ExecuteAsync(
            ct => _devices.AddAsync(
                descriptor.Id,
                () => _deviceFactory.Create(descriptor, this, GetProtocolAccessor(descriptor.Protocol), _recoveryPolicyHandler),
                ct),
            cancellationToken);
    }
    public ValueTask<bool> RemoveDeviceAsync(DeviceId deviceId, CancellationToken cancellationToken = default)
    {
        return _lifecycleStateMachine.ExecuteAsync(
            ct => _devices.RemoveAsync(deviceId, ct),
            cancellationToken);
    }


    private async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (_gatewayLifecycleState.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("端口启动失败, 网关未启动");

        await _transportConnection.StartAsync(cancellationToken);
        await _deviceBatchExecutor.StartAsync(_devices.Values, cancellationToken);
    }
    private async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        List<Exception> errors = [];

        try
        {
            await _deviceBatchExecutor.StopAsync(_devices.Values, cancellationToken);
        }
        catch (Exception ex)
        {
            errors.Add(ex);
            _logger.LogError(ex, "端口设备批处理停止失败", _transportConnection.Endpoint);
        }


        try { await _transportConnection.StopAsync(cancellationToken); }
        catch (Exception ex)
        {
            errors.Add(ex);
            _logger.LogError(ex, "端口传输器 {Endpoint} 停止失败", _transportConnection.Endpoint);
        }

        if (errors.Count == 0) return;
        throw new AggregateException($"端口 {Endpoint} 停止失败", errors);

    }
    private async ValueTask OnDisposeAsync()
    {
        // 释放设备集合
        try { await _devices.DisposeAsync(); }
        catch (Exception ex) { _logger.LogError(ex, "设备释放失败"); }

        // 批处理
        try { await _deviceBatchExecutor.DisposeAsync(_devices.Values); }
        catch (Exception ex) { _logger.LogError(ex, "设备释放失败"); }

        // 传输器
        try { await _transportConnection.DisposeAsync(); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "端口 {Endpoint} 传输器 释放失败", _transportConnection.Endpoint);
        }
    }



    private IProtocolAccessor GetProtocolAccessor(IProtocolDescriptor protocolDescriptor)
    {
        return _protocolAccessors.GetOrAdd(protocolDescriptor, x =>
            _protocolAccessorFactoryResolver.Resolve(x, _transportConnection));
    }
}