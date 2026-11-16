using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines;


internal sealed class StrictLifecycleStateMachine : LifecycleStateMachineBase
{
    private readonly ObservableLifecycleState _observableLifecycleState;
    private readonly Locker _locker;
    private readonly ILogger _logger;


    private StrictLifecycleStateMachine(ObservableLifecycleState observableLifecycleState, Locker locker, ILogger logger) : base(observableLifecycleState, new Executor(observableLifecycleState, locker))
    {
        _observableLifecycleState = observableLifecycleState;
        _locker = locker;
        _logger = logger;
    }
    public StrictLifecycleStateMachine(ILogger? logger = null) : this(new ObservableLifecycleState(), new Locker(1, 1), logger ?? NullLogger.Instance)
    {

    }

    /// <summary>
    /// 将状态调整至<see cref="LifecycleState.Starting"/> -> onStart() -> <see cref="LifecycleState.Running"/> <br/>
    /// 如果回调方法发生异常, 则调整为 <see cref="LifecycleState.Faulted"/>
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    [DebuggerStepThrough]
    public override async ValueTask StartAsync(Func<CancellationToken, ValueTask> onStart, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(State is LifecycleState.Disposed, this);
        ArgumentNullException.ThrowIfNull(onStart);


        using var _ = await _locker.WaiAsync( cancellationToken);
        if (State is LifecycleState.Running or LifecycleState.Starting)
        {
            _logger.LogWarning("已启动, 无法重复启动");
            return;
        }

        try
        {
            _observableLifecycleState.OnStarting();
            await onStart.InvokeWithTimeoutAsync(StartTimeout, cancellationToken);
            _observableLifecycleState.OnRunning();
        }
        catch
        {
            _observableLifecycleState.OnFaulted();
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
    public override async ValueTask StopAsync(Func<CancellationToken, ValueTask> onStop, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(State is LifecycleState.Disposed, this);
        ArgumentNullException.ThrowIfNull(onStop);

        using var _ = await _locker.WaiAsync(cancellationToken);
        ObjectDisposedException.ThrowIf(State is LifecycleState.Disposed, this);

        if (State is LifecycleState.Stopped or LifecycleState.Stopping)
        {
            _logger.LogWarning("已停止, 无法重复停止");
            return;
        }

        try
        {
            _observableLifecycleState.OnStopping();
            await onStop.InvokeWithTimeoutAsync(StopTimeout, cancellationToken);
            _observableLifecycleState.OnStopped();
        }
        catch
        {
            _observableLifecycleState.OnFaulted();
            throw;
        }
    }
    /// <summary>
    /// 将状态调整至 onDispose() -> <see cref="LifecycleState.Disposed"/>
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public override async ValueTask DisposeAsync(Func<ValueTask> onDispose)
    {
        if (State == LifecycleState.Disposed) return;
        if (onDispose is null) return;

        try
        {
            using var _ = await _locker.WaiAsync();

            if (State is LifecycleState.Disposed) return;
            _observableLifecycleState.OnDisposed();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "释放失败");
        }

        try
        {
            await onDispose.InvokeWithTimeoutAsync(DisposeTimeout);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "释放时发生错误");
        }
    }

    public override ValueTask DisposeAsync()
    {
        _locker.Dispose();
        _observableLifecycleState.Dispose();
        return base.DisposeAsync();
    }
}