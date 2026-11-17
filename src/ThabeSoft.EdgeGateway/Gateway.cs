using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway;


internal sealed class Gateway(in GatewayArgs dependencies) :
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