using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ThabeSoft.EdgeGateway.Lifecycles;


/// <summary>
/// 弹性生命周期基类。
/// </summary>
/// <remarks>
/// <para>
/// 本基类允许在组件已处于 <see cref="LifecycleState.Running"/> 状态时，**再次调用** <see cref="StartAsync"/>。
/// </para>
/// <para>
/// <b>核心适用场景：</b>
/// 适用于包含多个子组件（如调度器包含多个触发器、网关包含多个端口）的管理类。
/// 允许外部随时触发一次“全量扫描”，由子类在 <see cref="OnStartAsync"/> 中自行判断哪些子组件掉线了，
/// 从而实现“运行中自检与热修复（补启动）”。
/// </para>
/// <para>
/// <b>并发控制：</b>
/// 虽然允许重复调用，但内部依然使用 <see cref="SemaphoreSlim"/> 严格串行化。
/// 无论外部并发调用多少次，同一时刻绝对只有一个 <see cref="OnStartAsync"/> 在执行。
/// </para>
/// </remarks>
public abstract class ResilientLifecycleBase(ILogger logger) : ILifecycle, IAsyncDisposable
{
    private readonly SemaphoreSlim _stateChangeLock = new(1, 1);
    private readonly ILogger _logger = logger ?? NullLogger.Instance;
    private CancellationTokenSource? _lifecycleCts;

    protected ResilientLifecycleBase() : this(NullLogger.Instance) { }


    public LifecycleState State { get; private set; } = LifecycleState.Created;
    protected bool IsDisposed => State == LifecycleState.Disposed;


    public virtual async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        if (State is LifecycleState.Starting or LifecycleState.Disposed)
            return;

        await _stateChangeLock.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(State == LifecycleState.Disposed, this);


            var stateBeforeStart = State;
            if (State != LifecycleState.Running) State = LifecycleState.Starting;

            _lifecycleCts ??= CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                await OnStartAsync(_lifecycleCts.Token);
                State = LifecycleState.Running;
            }
            catch (Exception ex)
            {
                State = stateBeforeStart;

                if (stateBeforeStart != LifecycleState.Running)
                {
                    _lifecycleCts?.Dispose();
                    _lifecycleCts = null;
                }

                _logger.LogError(ex, "{Type} 启动/补启失败", GetType().Name);
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

            if (State is not (LifecycleState.Running or LifecycleState.Starting))
                return;

            var stateBeforeStop = State;
            State = LifecycleState.Stopping;

            if (_lifecycleCts is not null)
                await _lifecycleCts.CancelAsync();

            try
            {
                await OnStopAsync(cancellationToken);
                State = LifecycleState.Stopped;
            }
            catch (Exception ex)
            {
                State = stateBeforeStop;
                _logger.LogError(ex, "{Type} 停止失败", GetType().Name);
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

            if (State is LifecycleState.Running or LifecycleState.Starting)
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
                    _logger.LogError(ex, "{Type} 释放时停止服务发生异常", GetType().Name);
                }
                finally
                {
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