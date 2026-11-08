using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.Lifecycles;


public class LifecycleStateMachineA
{
    private readonly ILogger _logger;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private volatile LifecycleState _state = LifecycleState.Created;


    internal LifecycleStateMachineA(ILogger logger) => _logger = logger;


    public LifecycleState State => _state;
    public required TimeSpan StartTimeout { get; init; }
    public required TimeSpan StopTimeout { get; init; }
    public required TimeSpan DisposeTimeout { get; init; }
    public required TimeSpan LockTimeout { get; init; }


    /// <summary>
    /// 将状态调整至<see cref="LifecycleState.Starting"/> -> onStart() -> <see cref="LifecycleState.Running"/> <br/>
    /// 如果回调方法发生异常, 则调整为 <see cref="LifecycleState.Faulted"/>
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public async ValueTask StartAsync<TState>(TState state, Func<TState, CancellationToken, ValueTask> onStart, CancellationToken cancellationToken = default)
    {
        try
        {
            await AcquireLockAsync(cancellationToken);

            ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
            if (_state is LifecycleState.Running or LifecycleState.Starting) return;


            _state = LifecycleState.Starting;
            using var timeout = new CancellationTokenSource(StartTimeout);
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);

            try
            {
                await onStart(state, linked.Token);
                _state = LifecycleState.Running;
            }
            catch (OperationCanceledException) when (timeout.IsCancellationRequested)
            {
                _state = LifecycleState.Running;
                throw new TimeoutException($"启动超时（{StartTimeout}）");
            }
            catch
            {
                _state = LifecycleState.Faulted;
                throw;
            }
        }
        finally { ReleaseLock(); }
    }
    /// <summary>
    /// 将状态调整至<see cref="LifecycleState.Starting"/> -> onStart() -> <see cref="LifecycleState.Running"/> <br/>
    /// 如果回调方法发生异常, 则调整为 <see cref="LifecycleState.Faulted"/>
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public async ValueTask StartAsync(Func<CancellationToken, ValueTask> onStart, CancellationToken cancellationToken = default)
    {
        try
        {
            await AcquireLockAsync(cancellationToken);

            ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
            if (_state is LifecycleState.Running or LifecycleState.Starting) return;


            _state = LifecycleState.Starting;
            using var timeout = new CancellationTokenSource(StartTimeout);
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);

            try
            {
                await onStart(linked.Token);
                _state = LifecycleState.Running;
            }
            catch (OperationCanceledException) when (timeout.IsCancellationRequested)
            {
                _state = LifecycleState.Running;
                throw new TimeoutException($"启动超时（{StartTimeout}）");
            }
            catch
            {
                _state = LifecycleState.Faulted;
                throw;
            }
        }
        finally { ReleaseLock(); }
    }


    /// <summary>
    /// 将状态调整至<see cref="LifecycleState.Stopping"/> -> onStop() -> <see cref="LifecycleState.Stopped"/> <br/>
    /// 如果回调方法发生异常, 则调整为 <see cref="LifecycleState.Faulted"/>
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public async ValueTask StopAsync<TState>(TState state, Func<TState, CancellationToken, ValueTask> onStop, CancellationToken cancellationToken = default)
    {
        try
        {
            await AcquireLockAsync(cancellationToken);

            ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
            if (_state is LifecycleState.Stopped or LifecycleState.Stopping) return;

            _state = LifecycleState.Stopping;
            using var timeout = new CancellationTokenSource(StopTimeout);
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);
            try
            {
                await onStop(state, linked.Token);
                _state = LifecycleState.Stopped;
            }
            catch (OperationCanceledException) when (timeout.IsCancellationRequested)
            {
                _state = LifecycleState.Running;
                throw new TimeoutException($"停止超时（{StartTimeout}）");
            }
            catch
            {
                _state = LifecycleState.Faulted;
                throw;
            }
        }
        finally { ReleaseLock(); }
    }
    /// <summary>
    /// 将状态调整至<see cref="LifecycleState.Stopping"/> -> onStop() -> <see cref="LifecycleState.Stopped"/> <br/>
    /// 如果回调方法发生异常, 则调整为 <see cref="LifecycleState.Faulted"/>
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public async ValueTask StopAsync(Func<CancellationToken, ValueTask> onStop, CancellationToken cancellationToken = default)
    {
        try
        {
            await AcquireLockAsync(cancellationToken);

            ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
            if (_state is LifecycleState.Stopped or LifecycleState.Stopping) return;

            _state = LifecycleState.Stopping;
            using var timeout = new CancellationTokenSource(StopTimeout);
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);
            try
            {
                await onStop(linked.Token);
                _state = LifecycleState.Stopped;
            }
            catch (OperationCanceledException) when (timeout.IsCancellationRequested)
            {
                _state = LifecycleState.Running;
                throw new TimeoutException($"停止超时（{StartTimeout}）");
            }
            catch
            {
                _state = LifecycleState.Faulted;
                throw;
            }
        }
        finally { ReleaseLock(); }
    }


    /// <summary>
    /// 将状态调整至 onDispose() -> <see cref="LifecycleState.Disposed"/>
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public async ValueTask DisposeAsync<TState>(TState state, Func<TState, ValueTask> onDispose)
    {
        var lockAcquired = false;
        try
        {
            await AcquireLockAsync(CancellationToken.None);
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
            if (lockAcquired) ReleaseLock();
        }


        _state = LifecycleState.Disposed;
        try
        {
            var disposeTask = onDispose(state).AsTask();
            var timeoutTask = Task.Delay(DisposeTimeout);

            if (await Task.WhenAny(disposeTask, timeoutTask) == timeoutTask)
            {
                _logger.LogWarning("释放超时（{Timeout}），放弃等待", DisposeTimeout);
                return;
            }

            await disposeTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "释放时发生错误");
        }
    }
    /// <summary>
    /// 将状态调整至 onDispose() -> <see cref="LifecycleState.Disposed"/>
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public async ValueTask DisposeAsync(Func<ValueTask> onDispose)
    {
        var lockAcquired = false;
        try
        {
            await AcquireLockAsync(CancellationToken.None);
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
            if (lockAcquired) ReleaseLock();
        }


        _state = LifecycleState.Disposed;
        try
        {
            var disposeTask = onDispose().AsTask();
            var timeoutTask = Task.Delay(DisposeTimeout);

            if (await Task.WhenAny(disposeTask, timeoutTask) == timeoutTask)
            {
                _logger.LogWarning("释放超时（{Timeout}），放弃等待", DisposeTimeout);
                return;
            }

            await disposeTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "释放时发生错误");
        }
    }



    /// <summary>
    /// 共享状态锁执行方法
    /// </summary>
    public void Execute(Action action)
    {
        try
        {
            AcquireLock();

            ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
            action();
        }
        finally
        {
            ReleaseLock();
        }
    }
    /// <summary>
    /// 共享状态锁执行方法
    /// </summary>
    public void Execute<TState>(TState state, Action<TState> action)
    {
        try
        {
            AcquireLock();

            ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
            action(state);
        }
        finally
        {
            ReleaseLock();
        }
    }

    /// <summary>
    /// 共享状态锁执行方法
    /// </summary>
    public TResult Execute<TResult>(Func<TResult> action)
    {
        try
        {
            AcquireLock();
            ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
            return action();
        }
        finally
        {
            ReleaseLock();
        }
    }
    /// <summary>
    /// 共享状态锁执行方法
    /// </summary>
    public TResult Execute<TState, TResult>(TState state, Func<TState, TResult> action)
    {
        try
        {
            AcquireLock();
            ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
            return action(state);
        }
        finally
        {
            ReleaseLock();
        }
    }



    /// <summary>
    /// 共享状态锁执行方法
    /// </summary>
    public async ValueTask ExecuteAsync(Func<CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
    {
        try
        {
            await AcquireLockAsync(cancellationToken);
            ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
            await action(cancellationToken);
        }
        finally
        {
            ReleaseLock();
        }
    }
    /// <summary>
    /// 共享状态锁执行方法
    /// </summary>
    public async ValueTask ExecuteAsync<TState>(TState state, Func<TState, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
    {
        try
        {
            await AcquireLockAsync(cancellationToken);
            ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
            await action(state, cancellationToken);
        }
        finally
        {
            ReleaseLock();
        }
    }


    /// <summary>
    /// 共享状态锁执行方法
    /// </summary>
    public async ValueTask<TResult> ExecuteAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> action, CancellationToken cancellationToken = default)
    {
        try
        {
            await AcquireLockAsync(cancellationToken);
            ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
            return await action(cancellationToken);
        }
        finally
        {
            ReleaseLock();
        }
    }
    /// <summary>
    /// 共享状态锁执行方法
    /// </summary>
    public async ValueTask<TResult> ExecuteAsync<TState, TResult>(TState state, Func<TState, CancellationToken, ValueTask<TResult>> action, CancellationToken cancellationToken = default)
    {
        try
        {
            await AcquireLockAsync(cancellationToken);
            ObjectDisposedException.ThrowIf(_state is LifecycleState.Disposed, this);
            return await action(state, cancellationToken);
        }
        finally
        {
            ReleaseLock();
        }
    }



    /// <summary>
    /// 获取锁
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    private async ValueTask AcquireLockAsync(CancellationToken cancellationToken)
    {
        using var timeout = new CancellationTokenSource(LockTimeout);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);
        try
        {
            await _lock.WaitAsync(linked.Token);
        }
        catch (OperationCanceledException ex) when (timeout.IsCancellationRequested)
        {
            throw new TimeoutException($"获取状态机锁超时（{LockTimeout}）", ex);
        }
    }
    /// <summary>
    /// 获取锁
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    private void AcquireLock()
    {
        using var timeout = new CancellationTokenSource(LockTimeout);
        try
        {
            _lock.Wait(timeout.Token);
        }
        catch (OperationCanceledException ex) when (timeout.IsCancellationRequested)
        {
            throw new TimeoutException($"获取状态机锁超时（{LockTimeout}）", ex);
        }
    }
    /// <summary>
    /// 释放锁
    /// </summary>
    private void ReleaseLock()
    {
        _lock.Release();
    }
}