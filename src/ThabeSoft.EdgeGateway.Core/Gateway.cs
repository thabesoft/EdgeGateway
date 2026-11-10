using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway;


internal sealed class Gateway : IGateway, IAsyncDisposable
{
    private readonly PortFactory _portFactory;

    private readonly ILifecycleKeyedCollection<ConnectionEndpoint, Port> _ports;
    private readonly ILifecycleBatchExecutor _batchExecutor;
    private readonly IRetryableLifecycleStateMachine _stateMachine;


    public Gateway(PortFactory portFactory, ILogger<Gateway>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(portFactory);

        _portFactory = portFactory;
        logger ??= NullLogger<Gateway>.Instance;

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
        return await _stateMachine.ExecuteAsync(
            ct => _ports.AddAsync(
                transportDescriptor.Endpoint,
                () => _portFactory.Create(transportDescriptor, this),
                ct),
            cancellationToken);
    }
    public ValueTask<bool> RemovePortAsync(ConnectionEndpoint connectionEndpoint, CancellationToken cancellationToken = default)
    {
        return _stateMachine.ExecuteAsync(
            ct => _ports.RemoveAsync(connectionEndpoint, ct),
            cancellationToken);
    }
}