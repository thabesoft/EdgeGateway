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

    private readonly ILifecycleDictionary<ConnectionEndpoint, Port> _ports;
    private readonly ILifecycleBatchExecutor _batchExecutor;
    private readonly IRetryableLifecycleStateMachine _stateMachine;


    public Gateway(PortFactory portFactory, IEventPublisher eventPublisher, ILogger<Gateway>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(portFactory);

        _portFactory = portFactory;
        this._eventPublisher = eventPublisher;
        _logger ??= NullLogger<Gateway>.Instance;

        _ports = Lifecycle.KeyedCollection<ConnectionEndpoint, Port>(this, logger);
        Ports = new ReadOnlyDictionaryView<ConnectionEndpoint, Port, IPort>(_ports.Items);

        _batchExecutor = Lifecycle.BatchExecutor(logger);
        _stateMachine = Lifecycle.RetryableStateMachine(logger);
    }


    public LifecycleState State => _stateMachine.State;
    public IReadOnlyDictionary<ConnectionEndpoint, IPort> Ports { get; }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(ct => _batchExecutor.StartAsync(_ports.Values, ct), cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(ct => _batchExecutor.StopAsync(_ports.Values, ct), cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(() => _batchExecutor.DisposeAsync(_ports.Values));



    public async ValueTask<IPort> AddPortAsync(ITransportDescriptor transportDescriptor, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("添加端口 ({Endpoint})", transportDescriptor.Endpoint);

        try
        {
            var port = await _stateMachine.ExecuteAsync(
                ct => _ports.AddAsync(
                    transportDescriptor.Endpoint,
                    () => _portFactory.Create(transportDescriptor, this),
                    ct),
                cancellationToken);

            _logger.LogDebug("端口已添加 ({Endpoint})", port.Endpoint);
            _eventPublisher.Publish(new GatewayPortAddedEvent(port));

            return port;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "端口 ({Endpoint}) 添加失败", transportDescriptor.Endpoint);
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
}