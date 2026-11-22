using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Primitives;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway;


internal sealed class Gateway(GatewayDependencies dependencies) :
    IGateway, IAsyncDisposable
{
    private readonly DisposeFlag _disposeFlag = new();


    public LifecycleState State => dependencies.StateMachine.State;
    public IReadOnlyDictionary<PortId, IPort> Ports => dependencies.PortManager.Items;


    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);
        return dependencies.StateMachine.TransitionToRunningAsync(dependencies.PortManager.StartAllAsync, cancellationToken);
    }
    public ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);
        return dependencies.StateMachine.TransitionToStoppedAsync(dependencies.PortManager.StopAllAsync, cancellationToken);
    }


    public async ValueTask<IPort> AddPortAsync(PortDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);

        return await dependencies.StateMachine.ExecuteAsync(ct =>
            dependencies.PortManager.AddAsync(descriptor, ct), cancellationToken);
    }
    public async ValueTask<bool> RemovePortAsync(PortId id, CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);

        return await dependencies.StateMachine.ExecuteAsync(ct =>
            dependencies.PortManager.RemoveAsync(id, ct), cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposeFlag.TryMarkDisposed()) return;
        await Safe.TryEexecuteAsync(dependencies.OnDispose, dependencies.Logger);
    }
}


internal sealed class GatewayDependencies
{
    // Gateway
    public required ILifecycleStateMachine StateMachine { get; init => field = value.Required(); }

    // Port
    public required PortManager PortManager { get; init => field = value.Required(); }


    // Services
    public required ILogger<Gateway> Logger { get; init => field = value.Required(); }
    public required Func<ValueTask> OnDispose { get; init => field = value.Required(); }
}