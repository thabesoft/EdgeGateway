using System.Diagnostics;

namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines;


/// <summary>
/// 生命周期状态机
/// </summary>
public interface ILifecycleStateMachine : ILifecycleStateable, IExecutor, IAsyncExecutor, ILifecycleStateNotifier, IAsyncDisposable
{
    ValueTask StartAsync(Func<CancellationToken, ValueTask> onStart, CancellationToken cancellationToken = default);
    ValueTask StopAsync(Func<CancellationToken, ValueTask> onStop, CancellationToken cancellationToken = default);
    ValueTask DisposeAsync(Func<ValueTask> onDispose);
}



internal abstract class LifecycleStateMachineBase(ObservableLifecycleState observableLifecycleState, Executor executor) :
    ILifecycleStateMachine
{
    public LifecycleState State => observableLifecycleState.State;
    public IObservable<LifecycleStateTransition> StateChanged => observableLifecycleState.StateChanged;
    public TimeSpan ExecuteTimeout => executor.ExecuteTimeout;



#if DEBUG
    public TimeSpan StartTimeout { get; init; } = TimeSpan.FromMinutes(5);
    public TimeSpan StopTimeout { get; init; } = TimeSpan.FromMinutes(5);
    public TimeSpan DisposeTimeout { get; init; } = TimeSpan.FromMinutes(5);
    public TimeSpan LockTimeout { get; init; } = TimeSpan.FromMinutes(5);
#else
    public TimeSpan StartTimeout { get; init; } = TimeSpan.FromSeconds(30);
    public TimeSpan StopTimeout { get; init; } = TimeSpan.FromSeconds(15);
    public TimeSpan DisposeTimeout { get; init; } = TimeSpan.FromSeconds(10);
    public TimeSpan LockTimeout { get; init; } = TimeSpan.FromSeconds(30);
#endif


    public abstract ValueTask StartAsync(Func<CancellationToken, ValueTask> onStart, CancellationToken cancellationToken = default);
    public abstract ValueTask StopAsync(Func<CancellationToken, ValueTask> onStop, CancellationToken cancellationToken = default);
    public abstract ValueTask DisposeAsync(Func<ValueTask> onDispose);
    public virtual ValueTask DisposeAsync() => DisposeAsync(() => ValueTask.CompletedTask);



    [DebuggerStepThrough]
    public ValueTask ExecuteAsync(
            Func<CancellationToken, ValueTask> action,
            CancellationToken cancellationToken = default)
        => executor.ExecuteAsync(action, cancellationToken);

    [DebuggerStepThrough]
    public ValueTask<TResult> ExecuteAsync<TResult>(
            Func<CancellationToken, ValueTask<TResult>> action,
            CancellationToken cancellationToken = default)
        => executor.ExecuteAsync(action, cancellationToken);

    [DebuggerStepThrough]
    public void Execute(Action action) =>
         executor.Execute(action);

    [DebuggerStepThrough]
    public TResult Execute<TResult>(Func<TResult> action) =>
        executor.Execute(action);
}