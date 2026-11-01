using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ThabeSoft.EdgeGateway.Lifecycles;


/// <summary>
/// 提供线程安全且排他（防重入）的生命周期基类。
/// </summary>
/// <remarks>
/// <para>
/// 该基类通过无锁的原子操作（<see cref="Interlocked"/>）维护运行状态，确保在同一时刻，
/// <see cref="OnStartAsync"/> 和 <see cref="OnStopAsync"/> 绝不会被并发或重复调用。
/// </para>
/// <para>
/// <b>适用场景：</b> 适用于管理独占型物理资源或后台任务的组件，例如：
/// <list type="bullet">
/// <item>TCP/UDP 网络连接（通道）</item>
/// <item>串口通信</item>
/// <item>底层硬件驱动、独占型后台服务</item>
/// </list>
/// </para>
/// <para>
/// <b>不适用场景：</b> 若你的组件需要支持“重复调用 Start 以补启动/重试”的幂等性逻辑，请勿使用此类。
/// </para>
/// <para>
/// <b>异常处理：</b> 如果在启动过程中发生异常，基类会自动将内部状态回滚为“未启动”，保证下次调用能够重新尝试。
/// </para>
/// </remarks>
public abstract class ConcurrentLifecycleBase(ILogger logger) : ILifecycle, IAsyncDisposable
{
    private readonly SemaphoreSlim _stateChangeLock = new(1, 1);
    private volatile CancellationTokenSource? _lifecycleCts;

    protected ConcurrentLifecycleBase() : this(NullLogger.Instance) { }


    public LifecycleState State { get; private set; }
    protected bool IsDisposed => State == LifecycleState.Disposed;
    protected bool IsRunning => State == LifecycleState.Running;



    public virtual async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        if (State is LifecycleState.Running or LifecycleState.Starting or LifecycleState.Disposed)
            return;


        await _stateChangeLock.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(State == LifecycleState.Disposed, this);
            if (State is not(LifecycleState.Created or LifecycleState.Stopped)) return;

            var stateBeforeStart = State;
            State = LifecycleState.Starting;
            _lifecycleCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                await OnStartAsync(_lifecycleCts.Token);
                State = LifecycleState.Running;
            }
            catch
            {
                State = stateBeforeStart;
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
        if (State is LifecycleState.Stopping or LifecycleState.Stopped or LifecycleState.Created or LifecycleState.Disposed)
            return;


        await _stateChangeLock.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(State == LifecycleState.Disposed, this);
            if (State is not LifecycleState.Running) return;

            var stateBeforeStop = State;
            State = LifecycleState.Stopping;

            if (_lifecycleCts is not null)
                await _lifecycleCts.CancelAsync();

            try
            {
                await OnStopAsync(cancellationToken);
                State = LifecycleState.Stopped;
            }
            catch
            {
                State = stateBeforeStop;
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
        if (State == LifecycleState.Disposed) return;


        await _stateChangeLock.WaitAsync();
        try
        {
            if (State == LifecycleState.Disposed) return;

            if (State == LifecycleState.Running)
            {
                State = LifecycleState.Stopping;

                if (_lifecycleCts is not null)
                    await _lifecycleCts.CancelAsync();

                try
                {
                    await OnStopAsync(CancellationToken.None);
                    State = LifecycleState.Stopped;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "释放失败, 停止失败");
                }
                finally
                {
                    _lifecycleCts?.Dispose();
                    _lifecycleCts = null;
                }
            }

            try { await OnDisposeAsync(); }
            catch (Exception ex) { logger.LogError(ex, ""); }

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