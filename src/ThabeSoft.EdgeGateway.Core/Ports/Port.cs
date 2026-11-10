using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class Port : IPort, IAsyncDisposable
{
    private readonly ITransportConnection _transportConnection;
    private readonly DeviceFactory _deviceFactory;
    private readonly ILifecycleStateable _gatewayLifecycleState;
    private readonly ILogger<Port> _logger;

    private readonly ILifecycleKeyedCollection<DeviceId, Device> _devices;
    private readonly ILifecycleBatchExecutor _batchExecutor;
    private readonly IRetryableLifecycleStateMachine _stateMachine;



    public Port(
        ITransportDescriptor transportDescriptor,
        TransportConnectionFactorySelector transportConnectionFactorySelector,
        DeviceFactory deviceFactory,
        ILifecycleStateable gatewayLifecycleState,
        ILogger<Port> logger)
    {
        _deviceFactory = deviceFactory;
        _gatewayLifecycleState = gatewayLifecycleState;
        _transportConnection = transportConnectionFactorySelector.Create(transportDescriptor);
        _logger = logger;

        _devices = Lifecycle.KeyedCollection<DeviceId, Device>(this, logger);
        Devices = new ReadOnlyDictionaryView<DeviceId, Device, IDevice>(_devices.Items);

        _batchExecutor = Lifecycle.BatchExecutor(logger);
        _stateMachine = Lifecycle.RetryableStateMachine(logger);
    }

    public ConnectionEndpoint Endpoint => _transportConnection.Endpoint;
    public IReadOnlyDictionary<DeviceId, IDevice> Devices { get; }
    public LifecycleState State => _stateMachine.State;


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);


    public async ValueTask<IDevice> AddDeviceAsync(DeviceDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        return await _stateMachine.ExecuteAsync(
            ct => _devices.AddAsync(
                descriptor.Id,
                () => _deviceFactory.Create(descriptor, this, _transportConnection),
                ct),
            cancellationToken);
    }
    public ValueTask<bool> RemoveDeviceAsync(DeviceId deviceId, CancellationToken cancellationToken = default)
    {
        return _stateMachine.ExecuteAsync(
            ct => _devices.RemoveAsync(deviceId, ct),
            cancellationToken);
    }


    private async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (_gatewayLifecycleState.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("端口启动失败, 网关未启动");

        await _transportConnection.StartAsync(cancellationToken);
        await _batchExecutor.StartAsync(_devices.Values, cancellationToken);
    }
    private async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        List<Exception> errors = [];

        try
        {
            await _batchExecutor.StopAsync(_devices.Values, cancellationToken);
        }
        catch (Exception ex)
        {
            errors.Add(ex);
        }

        try { await _transportConnection.StopAsync(cancellationToken); }
        catch (Exception ex)
        {
            errors.Add(ex);
            _logger.LogError(ex, "传输器 {Endpoint} 停止失败", _transportConnection.Endpoint);
        }

        if (errors.Count == 0) return;
        throw new AggregateException($"端口 {Endpoint} 停止失败", errors);

    }
    private async ValueTask OnDisposeAsync()
    {
        try { await _batchExecutor.DisposeAsync(_devices.Values); }
        catch (Exception ex) { _logger.LogError(ex, "设备释放失败"); }

        try
        {
            await _transportConnection.DisposeAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "端口 {Endpoint} 传输器 释放失败", _transportConnection.Endpoint);
        }
    }
}