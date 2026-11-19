using Microsoft.Extensions.Logging;
using System.Diagnostics;
using ThabeSoft.EdgeGateway.Lifecycles.Utilities;
using ThabeSoft.EdgeGateway.Primitives;

namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines.Retryable;


internal abstract class LifecycleStateMachineBase(
        ObservableLifecycleState lifecycleState,
        Executor executor,
        Locker locker,
        ILogger logger,
        Func<ValueTask> onDispose
    ) : ILifecycleStateMachine, IAsyncDisposable
{
    private DisposeFlag _disposeFlag = new();


    protected abstract TimeSpan StartTimeout { get; }
    protected abstract TimeSpan StopTimeout { get; }
    protected abstract TimeSpan TerminatedTimeout { get; }


    public LifecycleState State => lifecycleState.State;
    public IObservable<LifecycleStateTransition> StateChanged => lifecycleState.StateChanged;



    [DebuggerStepThrough]
    public async ValueTask TransitionToRunningAsync(Func<CancellationToken, ValueTask> onStart, CancellationToken cancellationToken = default)
    {
        if (!CheckCanTransitionToRunning()) return;
        using var _ = await locker.WaitAsync(cancellationToken);
        if (!CheckCanTransitionToRunning()) return;

        try
        {
            lifecycleState.OnStarting();

            await onStart.InvokeWithTimeoutAsync(StartTimeout, "启动", cancellationToken);

            lifecycleState.OnRunning();
            logger.LogTrace("已启动");
        }
        catch (Exception ex)
        {
            lifecycleState.OnFaulted();
            logger.LogError(ex, "启动失败");

            throw;
        }
    }
    [DebuggerStepThrough]
    public async ValueTask TransitionToStoppedAsync(Func<CancellationToken, ValueTask> onStop, CancellationToken cancellationToken = default)
    {
        if (!CheckCanTransitionToStopped()) return;
        using var _ = await locker.WaitAsync(cancellationToken);
        if (!CheckCanTransitionToStopped()) return;

        try
        {
            lifecycleState.OnStopping();
            await onStop.InvokeWithTimeoutAsync(StopTimeout, "停止", cancellationToken);

            lifecycleState.OnStopped();
            logger.LogTrace("已停止");
        }
        catch (Exception ex)
        {
            lifecycleState.OnFaulted();
            logger.LogError(ex, "停止失败");
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposeFlag.TryMarkDisposed()) return;

        lifecycleState.OnTerminating();
        await Safe.TryEexecuteAsync(onDispose, logger);
        lifecycleState.OnTerminated();
    }


    public void Execute(Action handler) =>
        executor.Execute(handler);
    public TResult Execute<TResult>(Func<TResult> handler) =>
        executor.Execute(handler);
    public ValueTask ExecuteAsync(Func<CancellationToken, ValueTask> handler, CancellationToken cancellationToken = default) =>
        executor.ExecuteAsync(handler, cancellationToken);
    public ValueTask<TResult> ExecuteAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> handler, CancellationToken cancellationToken = default) =>
        executor.ExecuteAsync(handler, cancellationToken);


    protected abstract bool CanTransitionToRunning();
    protected abstract bool CanTransitionToStopped();
    protected abstract bool CanTransitionToTerminated();



    private bool CheckCanTransitionToRunning()
    {
        _disposeFlag.ThrowIfDisposed(this);
        return CanTransitionToRunning();
    }

    private bool CheckCanTransitionToStopped()
    {
        _disposeFlag.ThrowIfDisposed(this);
        return CanTransitionToStopped();
    }
}