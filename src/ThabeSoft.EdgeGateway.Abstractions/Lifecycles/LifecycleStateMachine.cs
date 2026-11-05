using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ThabeSoft.EdgeGateway.Lifecycles;


/// <summary>
/// 状态机生命周期 <br/>
/// 标准流程: 创建->启动中->运行中->停止中->已停止 <br/>
/// 不启动流程: 创建->已释放, 创建->停止中->已停止 <br/>
/// 启动失败流程: 创建->启动中->失败 <br/>
/// 停止失败流程: 创建->停止中->失败 <br/>
/// 释放流程: 任意状态->已释放
/// </summary>
/// <param name="logger"></param>
public abstract class LifecycleStateMachine(ILogger logger) : ILifecycle, IAsyncDisposable
{
    private readonly SemaphoreSlim _stateChangeLock = new(1, 1);
    private volatile CancellationTokenSource? _lifecycleCts;

    protected LifecycleStateMachine() : this(NullLogger.Instance) { }


    public LifecycleState State { get; private set; } = LifecycleState.Stopped;
    protected bool IsDisposed => State == LifecycleState.Disposed;
    protected bool IsRunning => State == LifecycleState.Running;



    public virtual async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        if (State is LifecycleState.Running or LifecycleState.Starting or LifecycleState.Disposed)
            return;


        await _stateChangeLock.WaitAsync(cancellationToken);
        try
        {
            if (State is LifecycleState.Running or LifecycleState.Starting or LifecycleState.Disposed)
                return;

            // 启动中
            State = LifecycleState.Starting;
            _lifecycleCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                // 运行中
                await OnStartAsync(_lifecycleCts.Token);
                State = LifecycleState.Running;
            }
            catch
            {
                // 启动失败
                State = LifecycleState.Faulted;
                _lifecycleCts.Dispose();
                _lifecycleCts = null;

                throw;
            }
        }
        finally
        {
            _stateChangeLock.Release();
        }
    }
    public virtual async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        if (State is LifecycleState.Stopping or LifecycleState.Stopped or LifecycleState.Disposed)
            return;

        await _stateChangeLock.WaitAsync(cancellationToken);
        try
        {
            if (State is LifecycleState.Stopping or LifecycleState.Stopped or LifecycleState.Disposed)
                return;

            // 停止中
            State = LifecycleState.Stopping;
            if (_lifecycleCts is not null) await _lifecycleCts.CancelAsync();

            try
            {
                // 已停止
                await OnStopAsync(cancellationToken);
                State = LifecycleState.Stopped;
            }
            catch
            {
                // 停止失败
                State = LifecycleState.Faulted;
                throw;
            }
            finally
            {
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
        // 已释放
        if (State == LifecycleState.Disposed) return;

        await _stateChangeLock.WaitAsync();
        try
        {
            if (State == LifecycleState.Disposed) return;

            // 还在运行
            if(State == LifecycleState.Running)
            {
                // 停止中
                State = LifecycleState.Stopping;
                if (_lifecycleCts is not null) await _lifecycleCts.CancelAsync();

                try
                {
                    await OnStopAsync(CancellationToken.None);
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, "{Type} 释放时停止服务发生异常", GetType().Name);
                }
                finally
                {
                    State = LifecycleState.Stopped;
                    _lifecycleCts?.Dispose();
                    _lifecycleCts = null;
                }
            }

            // 已释放
            try { await OnDisposeAsync(); }
            catch (Exception ex) { logger.LogError(ex, "{Type} 释放资源发生异常", GetType().Name); }

            State = LifecycleState.Disposed;
        }
        finally
        {
            _stateChangeLock.Release();
        }

        GC.SuppressFinalize(this);
    }



    /// <summary>
    /// 如果内部抛出异常, 则启动失败
    /// </summary>
    protected abstract ValueTask OnStartAsync(CancellationToken cancellationToken);
    /// <summary>
    /// 如果内部抛出异常, 向上抛出
    /// </summary>
    protected abstract ValueTask OnStopAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 不应抛出异常
    /// </summary>
    protected virtual ValueTask OnDisposeAsync()
        => ValueTask.CompletedTask;
}