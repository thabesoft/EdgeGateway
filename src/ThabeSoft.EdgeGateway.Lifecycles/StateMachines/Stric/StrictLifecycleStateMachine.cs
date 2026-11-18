using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Stateable;
using ThabeSoft.EdgeGateway.StateMachines;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.StateMachines.Stric;


internal sealed class StrictLifecycleStateMachine(
        StrictLifecycleStateMachineDescriptor descriptor,
        ObservableLifecycleState lifecycleState,
        Executor executor,
        Locker locker,
        ILogger<StrictLifecycleStateMachine>? logger = null
    ) : ILifecycleStateMachine
{
    private readonly TimeSpan _startTimeout = descriptor.StartTimeout;
    private readonly TimeSpan _stopTimeout = descriptor.StopTimeout;
    private readonly TimeSpan _disposeTimeout = descriptor.DisposedTimeout;
    private readonly ILogger _logger = logger ?? NullLogger<StrictLifecycleStateMachine>.Instance;



    public LifecycleState State => lifecycleState.State;
    public IObservable<LifecycleStateTransition> StateChanged => lifecycleState.StateChanged;
    public Func<ValueTask>? DisposedCallback { get; init; }


    /// <summary>
    /// 将状态调整至<see cref="LifecycleState.Starting"/> -> onStart() -> <see cref="LifecycleState.Running"/> <br/>
    /// 如果回调方法发生异常, 则调整为 <see cref="LifecycleState.Faulted"/>
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    [DebuggerStepThrough]
    public async ValueTask StartAsync(Func<CancellationToken, ValueTask> onStart, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(State is LifecycleState.Disposed, this);
        ArgumentNullException.ThrowIfNull(onStart);


        using var _ = await locker.WaiAsync(cancellationToken);
        if (State is LifecycleState.Running or LifecycleState.Starting)
        {
            _logger.LogWarning("已启动, 无法重复启动");
            return;
        }

        try
        {
            lifecycleState.OnStarting();
            await onStart.InvokeWithTimeoutAsync(_startTimeout, cancellationToken);
            lifecycleState.OnRunning();
        }
        catch
        {
            lifecycleState.OnFaulted();
            throw;
        }
    }
    /// <summary>
    /// 将状态调整至<see cref="LifecycleState.Stopping"/> -> onStop() -> <see cref="LifecycleState.Stopped"/> <br/>
    /// 如果回调方法发生异常, 则调整为 <see cref="LifecycleState.Faulted"/>
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    [DebuggerStepThrough]
    public async ValueTask StopAsync(Func<CancellationToken, ValueTask> onStop, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(State is LifecycleState.Disposed, this);
        ArgumentNullException.ThrowIfNull(onStop);

        using var _ = await locker.WaiAsync(cancellationToken);
        ObjectDisposedException.ThrowIf(State is LifecycleState.Disposed, this);

        if (State is LifecycleState.Stopped or LifecycleState.Stopping)
        {
            _logger.LogWarning("已停止, 无法重复停止");
            return;
        }

        try
        {
            lifecycleState.OnStopping();
            await onStop.InvokeWithTimeoutAsync(_stopTimeout, cancellationToken);
            lifecycleState.OnStopped();
        }
        catch
        {
            lifecycleState.OnFaulted();
            throw;
        }
    }
    /// <summary>
    /// 将状态调整至 onDispose() -> <see cref="LifecycleState.Disposed"/>
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public async ValueTask DisposeAsync(Func<ValueTask> onDispose)
    {
        if (State == LifecycleState.Disposed) return;
        if (onDispose is null) return;

        try
        {
            using var _ = await locker.WaiAsync();

            if (State is LifecycleState.Disposed) return;
            lifecycleState.OnDisposed();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "释放失败");
        }

        try
        {
            await onDispose.InvokeWithTimeoutAsync(_disposeTimeout);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "释放时发生错误");
        }
    }
    // 释放状态机
    public async ValueTask DisposeAsync()
    {
        if (DisposedCallback is null) return;

        try { await DisposedCallback.Invoke(); }
        catch (Exception ex) { _logger.LogError(ex, "状态机释放时回调异常"); }
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