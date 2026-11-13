using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway;


internal sealed class Gateway : IGateway, IAsyncDisposable
{
    private readonly PortFactory _portFactory;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<Gateway> _logger;

    private readonly ILifecycleKeyedCollection<ConnectionEndpoint, Port> _ports;
    private readonly ILifecycleBatchExecutor _batchExecutor;
    private readonly ILifecycleStateMachine _stateMachine;


    public Gateway(
        PortFactory portFactory,
        IEventPublisher eventPublisher,
        ILogger<Gateway>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(portFactory);

        _portFactory = portFactory;
        _eventPublisher = eventPublisher;
        _logger ??= NullLogger<Gateway>.Instance;

        _ports = Lifecycle.KeyedCollection<ConnectionEndpoint, Port>(this, logger);
        Ports = new ReadOnlyDictionaryView<ConnectionEndpoint, Port, IPort>(_ports.Items);

        _batchExecutor = Lifecycle.BatchExecutor(logger);
        _stateMachine = Lifecycle.RetryableStateMachine(logger);
    }


    public LifecycleState State => _stateMachine.State;
    public IReadOnlyDictionary<ConnectionEndpoint, IPort> Ports { get; }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);



    public async ValueTask<IPort> AddPortAsync(PortDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("添加端口 ({Endpoint})", descriptor.Transport.Endpoint);

        try
        {
            var port = await _stateMachine.ExecuteAsync(
                ct => _ports.AddAsync(
                    descriptor.Transport.Endpoint,
                    () => _portFactory.Create(descriptor, this),
                    ct),
                cancellationToken);

            _logger.LogDebug("端口已添加 ({Endpoint})", port.Endpoint);

            return port;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "端口 ({Endpoint}) 添加失败", descriptor.Transport.Endpoint);
            throw;
        }
    }
    public async ValueTask<bool> RemovePortAsync(ConnectionEndpoint connectionEndpoint, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("删除端口 ({Endpoint})", connectionEndpoint);

        try
        {
            var isRemove = await _stateMachine.ExecuteAsync(
                ct => _ports.RemoveAsync(connectionEndpoint, ct),
                cancellationToken);

            if (isRemove)
            {
                _logger.LogDebug("端口 ({Endpoint}) 删除成功", connectionEndpoint);
                _eventPublisher.Publish(new GatewayPortRemovedEvent(connectionEndpoint));
            }
            else
            {
                _logger.LogDebug("端口 ({Endpoint}) 不存在，无需删除", connectionEndpoint);
            }

            return isRemove;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "端口 ({Endpoint}) 删除失败", connectionEndpoint);
            throw;
        }
    }


    private ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        return _batchExecutor.StartAsync(_ports.Values, cancellationToken);
    }
    private ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        return _batchExecutor.StopAsync(_ports.Values, cancellationToken);
    }
    private ValueTask OnDisposeAsync()
    {
        return _batchExecutor.DisposeAsync(_ports.Values);
    }
}