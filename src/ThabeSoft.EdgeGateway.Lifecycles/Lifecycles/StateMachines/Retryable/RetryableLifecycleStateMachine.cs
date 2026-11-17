using Microsoft.Extensions.Logging;
using System.Diagnostics;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines.Retryable;


internal sealed class RetryableLifecycleStateMachine(
        RetryableLifecycleStateMachineDescriptor descriptor,
        ObservableLifecycleState lifecycleState,
        Executor executor,
        Locker locker,
        ILogger<RetryableLifecycleStateMachine> logger
    ) : ILifecycleStateMachine
{

    private readonly TimeSpan _startTimeout = descriptor.StartTimeout;
    private readonly TimeSpan _stopTimeout = descriptor.StopTimeout;
    private readonly TimeSpan _disposeTimeout = descriptor.DisposedTimeout;


    public LifecycleState State => lifecycleState.State;
    public IObservable<LifecycleStateTransition> StateChanged => lifecycleState.StateChanged;
    public Func<ValueTask>? DisposedCallback { get; init; }



    [DebuggerStepThrough]
    public async ValueTask StartAsync(Func<CancellationToken, ValueTask> onStart, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(State is LifecycleState.Disposed, this);

        using var _ = await locker.WaiAsync(cancellationToken);

        ObjectDisposedException.ThrowIf(State is LifecycleState.Disposed, this);
        if (State is LifecycleState.Starting) return;

        try
        {
            lifecycleState.OnStarting();

            await onStart.InvokeWithTimeoutAsync(_startTimeout, "启动", cancellationToken);

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
    public async ValueTask StopAsync(Func<CancellationToken, ValueTask> onStop, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(State is LifecycleState.Disposed, this);


        using var _ = await locker.WaiAsync(cancellationToken);
        ObjectDisposedException.ThrowIf(State is LifecycleState.Disposed, this);
        if (State is LifecycleState.Stopping or LifecycleState.Stopped) return;

        try
        {
            lifecycleState.OnStopping();
            await onStop.InvokeWithTimeoutAsync(_stopTimeout, "停止", cancellationToken);

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
    public async ValueTask DisposeAsync(Func<ValueTask> onDispose)
    {
        using var _ = await locker.WaiAsync();
        if (State is LifecycleState.Disposed) return;
        lifecycleState.OnDisposed();

        try
        {
            await onDispose.InvokeWithTimeoutAsync(_disposeTimeout, "执行");
            logger.LogTrace("已释放");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "释放失败");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (DisposedCallback is null) return;

        try { await DisposedCallback.Invoke(); }
        catch (Exception ex) { logger.LogError(ex, "状态机释放回调执行失败"); }
    }


    public void Execute(Action handler) =>
        executor.Execute(handler);
    public TResult Execute<TResult>(Func<TResult> handler) =>
        executor.Execute(handler);
    public ValueTask ExecuteAsync(Func<CancellationToken, ValueTask> handler, CancellationToken cancellationToken = default) =>
        executor.ExecuteAsync(handler, cancellationToken);
    public ValueTask<TResult> ExecuteAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> handler, CancellationToken cancellationToken = default) =>
        executor.ExecuteAsync(handler, cancellationToken);
}