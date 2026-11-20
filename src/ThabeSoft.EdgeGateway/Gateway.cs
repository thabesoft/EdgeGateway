using Microsoft.Extensions.Logging;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Primitives;

namespace ThabeSoft.EdgeGateway;


internal sealed class Gateway(
        ILifecycleStateMachine _stateMachine,
        PortManager _portManager,
        ILogger<Gateway> _logger,
        Func<ValueTask> onDispose
    ) :
    IGateway, IAsyncDisposable
{
    private readonly DisposeFlag _disposeFlag = new();


    public LifecycleState State => _stateMachine.State;
    public IReadOnlyDictionary<PortId, IPort> Ports => _portManager.Items;


    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);
        return _stateMachine.TransitionToRunningAsync(_portManager.StartAllAsync, cancellationToken);
    }
    public ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);
        return _stateMachine.TransitionToStoppedAsync(_portManager.StopAllAsync, cancellationToken);
    }


    public async ValueTask<IPort> AddPortAsync(PortDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);

        return await _stateMachine.ExecuteAsync(ct =>
            _portManager.AddAsync(descriptor, ct), cancellationToken);
    }
    public async ValueTask<bool> RemovePortAsync(PortId id, CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);

        return await _stateMachine.ExecuteAsync(ct =>
            _portManager.RemoveAsync(id, ct), cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposeFlag.TryMarkDisposed()) return;
        await Safe.TryEexecuteAsync(onDispose, _logger);
    }
}