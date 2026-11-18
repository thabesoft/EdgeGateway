using System.Diagnostics;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.StateMachines;


internal sealed class Executor(
        ILifecycleStateable lifecycleStateable,
        Locker locker
    ) : IAsyncExecutor, IExecutor
{

#if DEBUG
    public TimeSpan ExecuteTimeout { get; init; } = TimeSpan.FromMinutes(5);
#else
    public TimeSpan ExecuteTimeout { get; init; } = TimeSpan.FromSeconds(30);
#endif


    [DebuggerStepThrough]
    public async ValueTask ExecuteAsync(
        Func<CancellationToken, ValueTask> action,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(lifecycleStateable.State is LifecycleState.Disposed, this);
        ArgumentNullException.ThrowIfNull(action);

        using var _ = await locker.WaiAsync(cancellationToken);

        ObjectDisposedException.ThrowIf(lifecycleStateable.State is LifecycleState.Disposed, this);
        await action.InvokeWithTimeoutAsync(ExecuteTimeout, "执行", cancellationToken);

    }

    [DebuggerStepThrough]
    public async ValueTask<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, ValueTask<TResult>> action,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(lifecycleStateable.State is LifecycleState.Disposed, this);
        ArgumentNullException.ThrowIfNull(action);

        using var _ = await locker.WaiAsync(cancellationToken);

        ObjectDisposedException.ThrowIf(lifecycleStateable.State is LifecycleState.Disposed, this);
        return await action.InvokeWithTimeoutAsync(ExecuteTimeout, "执行", cancellationToken);

    }

    [DebuggerStepThrough]
    public void Execute(Action action)
    {
        ObjectDisposedException.ThrowIf(lifecycleStateable.State is LifecycleState.Disposed, this);
        ArgumentNullException.ThrowIfNull(action);

        using var _ = locker.Wait();
        ObjectDisposedException.ThrowIf(lifecycleStateable.State is LifecycleState.Disposed, this);

        action();
    }

    [DebuggerStepThrough]
    public TResult Execute<TResult>(Func<TResult> action)
    {
        ObjectDisposedException.ThrowIf(lifecycleStateable.State is LifecycleState.Disposed, this);
        ArgumentNullException.ThrowIfNull(action);

        using var _ = locker.Wait();
        ObjectDisposedException.ThrowIf(lifecycleStateable.State is LifecycleState.Disposed, this);

        return action();
    }
}