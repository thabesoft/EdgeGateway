using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ThabeSoft.EdgeGateway.Extensions;

namespace ThabeSoft.EdgeGateway.Lifecycles;

/// <summary>
/// 可以多次连续启动和多次连续停止的状态机
/// </summary>
public interface IRetryableLifecycleStateMachine :
    ILifecycleStateMachine,
    IExecutor,
    IAsyncExecutor,
    IAsyncDisposable;


internal sealed class RetryableLifecycleStateMachine(ILogger? logger = null) :
    IRetryableLifecycleStateMachine
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly ILogger _logger = logger ?? NullLogger.Instance;
    private volatile LifecycleState _state = LifecycleState.Created;


    public LifecycleState State => _state;
    public TimeSpan StartTimeout { get; init; } = TimeSpan.FromSeconds(30);
    public TimeSpan StopTimeout { get; init; } = TimeSpan.FromSeconds(15);
    public TimeSpan DisposeTimeout { get; init; } = TimeSpan.FromSeconds(10);
    public TimeSpan ExecuteTimeout { get; init; } = TimeSpan.FromSeconds(30);
    public TimeSpan LockTimeout { get; init; } = TimeSpan.FromSeconds(30);



    public async ValueTask StartAsync(Func<CancellationToken, ValueTask> onStart, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);

        await _lock.WaitWithTimeoutAsync(LockTimeout, cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
            if (_state is LifecycleState.Starting) return;

            try
            {
                _state = LifecycleState.Starting;
                await onStart.InvokeWithTimeoutAsync(StopTimeout, "启动", cancellationToken);
                _state = LifecycleState.Running;

                _logger.LogTrace("已启动");
            }
            catch (Exception ex)
            {
                _state = LifecycleState.Faulted;
                _logger.LogError(ex, "启动失败");

                throw;
            }
        }
        finally { _lock.Release(); }
    }
    public async ValueTask StopAsync(Func<CancellationToken, ValueTask> onStop, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);


        await _lock.WaitWithTimeoutAsync(LockTimeout, cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
            if (_state is LifecycleState.Stopping or LifecycleState.Stopped) return;

            try
            {
                _state = LifecycleState.Stopping;
                await onStop.InvokeWithTimeoutAsync(StopTimeout, "停止", cancellationToken);
                _state = LifecycleState.Stopped;

                _logger.LogTrace("已停止");
            }
            catch (Exception ex)
            {
                _state = LifecycleState.Faulted;
                _logger.LogError(ex, "停止失败");
                throw;
            }
        }
        finally { _lock.Release(); }
    }
    public async ValueTask DisposeAsync(Func<ValueTask> onDispose)
    {
        await _lock.WaitWithTimeoutAsync(LockTimeout);
        try
        {
            if (_state is LifecycleState.Disposed) return;
            _state = LifecycleState.Disposed;
        }
        finally { _lock.Release(); }

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
    public ValueTask DisposeAsync() => DisposeAsync(() => ValueTask.CompletedTask);



    public async ValueTask ExecuteAsync(
        Func<CancellationToken, ValueTask> action,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);

        await _lock.WaitWithTimeoutAsync(LockTimeout, cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
            await action.InvokeWithTimeoutAsync(ExecuteTimeout, "执行", cancellationToken);
        }
        finally { _lock.Release(); }
    }
    public async ValueTask<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, ValueTask<TResult>> action,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);

        await _lock.WaitWithTimeoutAsync(LockTimeout, cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
            return await action.InvokeWithTimeoutAsync(ExecuteTimeout, "执行", cancellationToken);
        }
        finally { _lock.Release(); }
    }

    public void Execute(Action action)
    {
        ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
        ArgumentNullException.ThrowIfNull(action);

        try
        {
            _lock.Wait(LockTimeout);
            action();
        }
        finally
        {
            _lock.Release();
        }
    }
    public TResult Execute<TResult>(Func<TResult> action)
    {
        ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
        ArgumentNullException.ThrowIfNull(action);

        try
        {
            _lock.Wait(LockTimeout);
            return action();
        }
        finally
        {
            _lock.Release();
        }
    }
}