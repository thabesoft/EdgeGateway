using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines;


internal sealed class RetryableLifecycleStateMachine : LifecycleStateMachineBase
{
    private readonly ObservableLifecycleState _observableLifecycleState;
    private readonly Locker _locker;
    private readonly ILogger _logger;


    private RetryableLifecycleStateMachine(ObservableLifecycleState observableLifecycleState, Locker locker, ILogger logger) : base(observableLifecycleState, new Executor(observableLifecycleState, locker))
    {
        _observableLifecycleState = observableLifecycleState;
        _locker = locker;
        _logger = logger;
    }
    public RetryableLifecycleStateMachine(ILogger? logger = null) : this(new ObservableLifecycleState(), new Locker(1, 1), logger ?? NullLogger.Instance)
    {
    }


    [DebuggerStepThrough]
    public override async ValueTask StartAsync(Func<CancellationToken, ValueTask> onStart, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(State is LifecycleState.Disposed, this);

        using var _ = await _locker.WaiAsync(cancellationToken);

        ObjectDisposedException.ThrowIf(State is LifecycleState.Disposed, this);
        if (State is LifecycleState.Starting) return;

        try
        {
            _observableLifecycleState.OnStarting();

            await onStart.InvokeWithTimeoutAsync(StopTimeout, "启动", cancellationToken);

            _observableLifecycleState.OnRunning();
            _logger.LogTrace("已启动");
        }
        catch (Exception ex)
        {
            _observableLifecycleState.OnFaulted();
            _logger.LogError(ex, "启动失败");

            throw;
        }
    }
    [DebuggerStepThrough]
    public override async ValueTask StopAsync(Func<CancellationToken, ValueTask> onStop, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(State is LifecycleState.Disposed, this);


        using var _ = await _locker.WaiAsync(cancellationToken);
        ObjectDisposedException.ThrowIf(State is LifecycleState.Disposed, this);
        if (State is LifecycleState.Stopping or LifecycleState.Stopped) return;

        try
        {
            _observableLifecycleState.OnStopping();
            await onStop.InvokeWithTimeoutAsync(StopTimeout, "停止", cancellationToken);

            _observableLifecycleState.OnStopped();
            _logger.LogTrace("已停止");
        }
        catch (Exception ex)
        {
            _observableLifecycleState.OnFaulted();
            _logger.LogError(ex, "停止失败");
            throw;
        }
    }
    public override async ValueTask DisposeAsync(Func<ValueTask> onDispose)
    {
        using var _ = await _locker.WaiAsync();
        if (State is LifecycleState.Disposed) return;
        _observableLifecycleState.OnDisposed();

        try
        {
            await onDispose.InvokeWithTimeoutAsync(ExecuteTimeout, "执行");
            _logger.LogTrace("已释放");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "释放失败");
        }
    }

    public override ValueTask DisposeAsync()
    {
        _locker.Dispose();
        _observableLifecycleState.Dispose();
        return base.DisposeAsync();
    }
}