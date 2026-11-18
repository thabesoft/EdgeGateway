using System.Diagnostics;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;

namespace ThabeSoft.EdgeGateway.StateMachines;

internal abstract class LifecycleStateMachineBase : IAsyncExecutor, IExecutor
{
    protected abstract Executor Executor { get; }


    [DebuggerStepThrough]
    public ValueTask ExecuteAsync(
            Func<CancellationToken, ValueTask> action,
            CancellationToken cancellationToken = default)
        => Executor.ExecuteAsync(action, cancellationToken);

    [DebuggerStepThrough]
    public ValueTask<TResult> ExecuteAsync<TResult>(
            Func<CancellationToken, ValueTask<TResult>> action,
            CancellationToken cancellationToken = default)
        => Executor.ExecuteAsync(action, cancellationToken);

    [DebuggerStepThrough]
    public void Execute(Action action) =>
         Executor.Execute(action);

    [DebuggerStepThrough]
    public TResult Execute<TResult>(Func<TResult> action) =>
        Executor.Execute(action);
}