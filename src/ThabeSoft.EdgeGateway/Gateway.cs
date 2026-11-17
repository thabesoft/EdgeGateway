using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway;


internal sealed class Gateway(
        ILifecycleStateMachine lifecycleStateMachine,
        PortManager portManager,
        ILogger<Gateway> logger
    ) : IGateway, IAsyncDisposable
{
    public LifecycleState State => lifecycleStateMachine.State;
    public IReadOnlyDictionary<PortId, IPort> Ports => portManager.Items;
    public Func<ValueTask>? DisposedCallback { get; init; }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        lifecycleStateMachine.StartAsync(portManager.StartAllAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        lifecycleStateMachine.StopAsync(portManager.StopAllAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        lifecycleStateMachine.DisposeAsync(OnDisposeAsync);



    public async ValueTask<IPort> AddPortAsync(PortDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        return await lifecycleStateMachine.ExecuteAsync(ct =>
            portManager.AddAsync(descriptor, ct), cancellationToken);
    }
    public async ValueTask<bool> RemovePortAsync(PortId id, CancellationToken cancellationToken = default)
    {
        return await lifecycleStateMachine.ExecuteAsync(ct =>
            portManager.RemoveAsync(id, ct), cancellationToken);
    }

    private async ValueTask OnDisposeAsync()
    {
        if (DisposedCallback is null) return;

        try { await DisposedCallback.Invoke(); }
        catch (Exception ex) { logger.LogError(ex, "网关释放回调执行异常"); }
    }
}