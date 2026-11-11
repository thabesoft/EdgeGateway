using System.Diagnostics;

namespace ThabeSoft.EdgeGateway.Lifecycles.Internal;


internal static class TimeoutExtensions
{
    extension(SemaphoreSlim semaphoreSlim)
    {
        [DebuggerHidden]
        public ValueTask WaitWithTimeoutAsync(TimeSpan timeout, string operation, CancellationToken cancellationToken = default) =>
            ExecuteAsync(async ct => await semaphoreSlim.WaitAsync(ct), timeout, operation, cancellationToken);

        [DebuggerHidden]
        public ValueTask WaitWithTimeoutAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
            semaphoreSlim.WaitWithTimeoutAsync(timeout, string.Empty, cancellationToken);

    }


    extension(Func<CancellationToken, ValueTask> action)
    {
        [DebuggerHidden]
        public ValueTask InvokeWithTimeoutAsync(TimeSpan timeout, string operation, CancellationToken cancellationToken = default) =>
             ExecuteAsync(action, timeout, operation, cancellationToken);

        [DebuggerHidden]
        public ValueTask InvokeWithTimeoutAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
            action.InvokeWithTimeoutAsync(timeout, string.Empty, cancellationToken);
    }

    extension<TResult>(Func<CancellationToken, ValueTask<TResult>> action)
    {
        [DebuggerHidden]
        public ValueTask<TResult> InvokeWithTimeoutAsync(TimeSpan timeout, string operation, CancellationToken cancellationToken = default) =>
            ExecuteAsync(action, timeout, operation, cancellationToken);

        [DebuggerHidden]
        public ValueTask<TResult> InvokeWithTimeoutAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
            action.InvokeWithTimeoutAsync(timeout, string.Empty, cancellationToken);
    }

    extension(Func<ValueTask> action)
    {
        [DebuggerHidden]
        public ValueTask InvokeWithTimeoutAsync(TimeSpan timeout, string operation) =>
            ExecuteAsync(action, timeout, operation);

        [DebuggerHidden]
        public ValueTask InvokeWithTimeoutAsync(TimeSpan timeout) =>
            action.InvokeWithTimeoutAsync(timeout, string.Empty);
    }


    [DebuggerHidden]
    public static async ValueTask ExecuteAsync(
        Func<CancellationToken, ValueTask> action,
        TimeSpan timeout,
        string operation,
        CancellationToken cancellationToken = default)
    {
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        try
        {
            await action(linked.Token);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            throw new TimeoutException($"{operation ?? "获取锁"}超时（{timeout}）");
        }
    }

    [DebuggerHidden]
    public static async ValueTask<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, ValueTask<TResult>> action,
        TimeSpan timeout,
        string operation,
        CancellationToken cancellationToken = default)
    {
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        try
        {
            return await action(linked.Token);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            throw new TimeoutException($"{operation ?? "获取锁"}超时（{timeout}）");
        }
    }

    [DebuggerHidden]
    public static async ValueTask ExecuteAsync(
        Func<ValueTask> action,
        TimeSpan timeout,
        string operation)
    {
        var actionTask = action().AsTask();
        var timeoutTask = Task.Delay(timeout);

        var completed = await Task.WhenAny(actionTask, timeoutTask);

        if (completed == timeoutTask)
        {
            throw new TimeoutException($"{operation}超时（{timeout}）");
        }

        await actionTask;
    }
}