using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ThabeSoft.EdgeGateway.Lifecycles;


/// <summary>
/// 允许多次启动和停止的状态机
/// </summary>
public abstract class RetryableLifecycle(ILogger logger) : ILifecycle, IAsyncDisposable
{
    private readonly SemaphoreSlim _stateChangeLock = new(1, 1);
    private readonly ILogger _logger = logger ?? NullLogger.Instance;
    private CancellationTokenSource? _lifecycleCts;

    protected RetryableLifecycle() : this(NullLogger.Instance) { }


    public LifecycleState State { get; private set; } = LifecycleState.Stopped;
    protected bool IsRunning => State == LifecycleState.Running;
    protected bool IsStopped => State == LifecycleState.Stopped;



    public virtual async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(State == LifecycleState.Disposed, this);
        if (State is LifecycleState.Starting)
            return;

        await _stateChangeLock.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(State == LifecycleState.Disposed, this);

            // 启动中或者已释放 不在启动
            if (State is LifecycleState.Starting or LifecycleState.Disposed)
                return;

            // 启动中
            State = LifecycleState.Starting;
            _lifecycleCts ??= CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                await OnStartAsync(_lifecycleCts.Token);
            }
            finally
            {
                // 已启动
                State = LifecycleState.Running;
            }
        }
        finally
        {
            _stateChangeLock.Release();
        }
    }
    public virtual async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(State == LifecycleState.Disposed, this);

        // 停止中或已释放
        if (State is LifecycleState.Stopping)
            return;

        await _stateChangeLock.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(State == LifecycleState.Disposed, this);

            // 停止中或已释放
            if (State is LifecycleState.Stopping)
                return;


            // 停止中
            State = LifecycleState.Stopping;
            if (_lifecycleCts is not null) await _lifecycleCts.CancelAsync();

            try
            {
                await OnStopAsync(cancellationToken);
            }
            finally
            {
                // 已停止
                State = LifecycleState.Stopped;

                _lifecycleCts?.Dispose();
                _lifecycleCts = null;
            }
        }
        finally
        {
            _stateChangeLock.Release();
        }
    }
    public virtual async ValueTask DisposeAsync()
    {
        if (State == LifecycleState.Disposed) return;

        await _stateChangeLock.WaitAsync();
        try
        {
            if (State == LifecycleState.Disposed) return;

            // 还在运行中
            if (State is LifecycleState.Running or LifecycleState.Starting)
            {
                // 停止中
                State = LifecycleState.Stopping;
                if (_lifecycleCts is not null) await _lifecycleCts.CancelAsync();

                try
                {
                    await OnStopAsync(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{Type} 释放时停止服务发生异常", GetType().Name);
                }
                finally
                {
                    State = LifecycleState.Stopped;
                    _lifecycleCts?.Dispose();
                    _lifecycleCts = null;
                }
            }

            try { await OnDisposeAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "{Type} 释放资源发生异常", GetType().Name); }

            State = LifecycleState.Disposed;
        }
        finally
        {
            _stateChangeLock.Release();
        }

        GC.SuppressFinalize(this);
    }


    protected abstract ValueTask OnStartAsync(CancellationToken cancellationToken);
    protected abstract ValueTask OnStopAsync(CancellationToken cancellationToken);
    protected virtual ValueTask OnDisposeAsync() => ValueTask.CompletedTask;
}