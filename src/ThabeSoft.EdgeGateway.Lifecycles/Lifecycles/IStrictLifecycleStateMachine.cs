using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.StateMachines;

namespace ThabeSoft.EdgeGateway.Lifecycles;


/// <summary>
/// 严格生命周期状态机, 不可连续重复启动, 不可连续重复停止
/// </summary>
public interface IStrictLifecycleStateMachine :
    ILifecycleStateMachine,
    IExecutor,
    IAsyncExecutor,
    IAsyncDisposable;


internal sealed class StrictLifecycleStateMachine(ILogger? logger = null) :
    IStrictLifecycleStateMachine
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly ILogger _logger = logger ?? NullLogger.Instance;
    private volatile LifecycleState _state = LifecycleState.Created;

    public LifecycleState State => _state;

#if DEBUG
    public TimeSpan StartTimeout { get; init; } = TimeSpan.FromMinutes(5);
    public TimeSpan StopTimeout { get; init; } = TimeSpan.FromMinutes(5);
    public TimeSpan DisposeTimeout { get; init; } = TimeSpan.FromMinutes(5);
    public TimeSpan ExecuteTimeout { get; init; } = TimeSpan.FromMinutes(5);
    public TimeSpan LockTimeout { get; init; } = TimeSpan.FromMinutes(5);
#else
    public TimeSpan StartTimeout { get; init; } = TimeSpan.FromSeconds(30);
    public TimeSpan StopTimeout { get; init; } = TimeSpan.FromSeconds(15);
    public TimeSpan DisposeTimeout { get; init; } = TimeSpan.FromSeconds(10);
    public TimeSpan ExecuteTimeout { get; init; } = TimeSpan.FromSeconds(30);
    public TimeSpan LockTimeout { get; init; } = TimeSpan.FromSeconds(30);
#endif


    /// <summary>
    /// 将状态调整至<see cref="LifecycleState.Starting"/> -> onStart() -> <see cref="LifecycleState.Running"/> <br/>
    /// 如果回调方法发生异常, 则调整为 <see cref="LifecycleState.Faulted"/>
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    [DebuggerStepThrough]
    public async ValueTask StartAsync(Func<CancellationToken, ValueTask> onStart, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
        ArgumentNullException.ThrowIfNull(onStart);


        await _lock.WaitWithTimeoutAsync(LockTimeout, cancellationToken);
        try
        {
            if (_state is LifecycleState.Running or LifecycleState.Starting)
                throw new InvalidOperationException("已启动, 无法重复启动");

            try
            {
                _state = LifecycleState.Starting;
                await onStart.InvokeWithTimeoutAsync(StartTimeout, cancellationToken);
                _state = LifecycleState.Running;
            }
            catch
            {
                _state = LifecycleState.Faulted;
                throw;
            }
        }
        finally { _lock.Release(); }
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
        ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
        ArgumentNullException.ThrowIfNull(onStop);

        await _lock.WaitWithTimeoutAsync(LockTimeout, cancellationToken);
        try
        {
            if (_state is LifecycleState.Stopped or LifecycleState.Stopping)
                throw new InvalidOperationException("已停止, 无法重复停止");

            try
            {
                _state = LifecycleState.Stopping;
                await onStop.InvokeWithTimeoutAsync(StopTimeout, cancellationToken);
                _state = LifecycleState.Stopped;
            }
            catch
            {
                _state = LifecycleState.Faulted;
                throw;
            }
        }
        finally { _lock.Release(); }
    }
    /// <summary>
    /// 将状态调整至 onDispose() -> <see cref="LifecycleState.Disposed"/>
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public async ValueTask DisposeAsync(Func<ValueTask> onDispose)
    {
        if (_state == LifecycleState.Disposed) return;
        if (onDispose is null) return;

        var lockAcquired = false;
        try
        {
            await _lock.WaitWithTimeoutAsync(LockTimeout);
            lockAcquired = true;

            if (_state is LifecycleState.Disposed) return;
            _state = LifecycleState.Disposed;
        }
        catch (TimeoutException)
        {
            _logger.LogWarning("释放时获取状态机锁超时，跳过状态标记，继续释放");
        }
        finally
        {
            if (lockAcquired) _lock.Release();
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

    public ValueTask DisposeAsync() => DisposeAsync(() => ValueTask.CompletedTask);


    /// <summary>
    /// 共享状态锁执行方法
    /// </summary>
    [DebuggerStepThrough]
    public async ValueTask ExecuteAsync(Func<CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
        ArgumentNullException.ThrowIfNull(action);

        try
        {
            await _lock.WaitWithTimeoutAsync(LockTimeout, cancellationToken);
            await action.InvokeWithTimeoutAsync(ExecuteTimeout, cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }
    /// <summary>
    /// 共享状态锁执行方法
    /// </summary>
    [DebuggerStepThrough]
    public async ValueTask<TResult> ExecuteAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> action, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
        ArgumentNullException.ThrowIfNull(action);

        try
        {
            await _lock.WaitWithTimeoutAsync(LockTimeout, cancellationToken);
            return await action.InvokeWithTimeoutAsync(ExecuteTimeout, cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }

    [DebuggerStepThrough]
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

    [DebuggerStepThrough]
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