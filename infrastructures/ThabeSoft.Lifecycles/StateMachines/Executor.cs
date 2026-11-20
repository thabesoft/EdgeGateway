using System.Diagnostics;
using ThabeSoft.Lifecycles.Utilities;

namespace ThabeSoft.Lifecycles.StateMachines;


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
        ObjectDisposedException.ThrowIf(lifecycleStateable.State is LifecycleState.Terminated, this);
        ArgumentNullException.ThrowIfNull(action);

        using var _ = await locker.WaitAsync(cancellationToken);

        ObjectDisposedException.ThrowIf(lifecycleStateable.State is LifecycleState.Terminated, this);
        await action.InvokeWithTimeoutAsync(ExecuteTimeout, "执行", cancellationToken);

    }

    [DebuggerStepThrough]
    public async ValueTask<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, ValueTask<TResult>> action,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(lifecycleStateable.State is LifecycleState.Terminated, this);
        ArgumentNullException.ThrowIfNull(action);

        using var _ = await locker.WaitAsync(cancellationToken);

        ObjectDisposedException.ThrowIf(lifecycleStateable.State is LifecycleState.Terminated, this);
        return await action.InvokeWithTimeoutAsync(ExecuteTimeout, "执行", cancellationToken);

    }

    [DebuggerStepThrough]
    public void Execute(Action action)
    {
        ObjectDisposedException.ThrowIf(lifecycleStateable.State is LifecycleState.Terminated, this);
        ArgumentNullException.ThrowIfNull(action);

        using var _ = locker.Wait();
        ObjectDisposedException.ThrowIf(lifecycleStateable.State is LifecycleState.Terminated, this);

        action();
    }

    [DebuggerStepThrough]
    public TResult Execute<TResult>(Func<TResult> action)
    {
        ObjectDisposedException.ThrowIf(lifecycleStateable.State is LifecycleState.Terminated, this);
        ArgumentNullException.ThrowIfNull(action);

        using var _ = locker.Wait();
        ObjectDisposedException.ThrowIf(lifecycleStateable.State is LifecycleState.Terminated, this);

        return action();
    }
}