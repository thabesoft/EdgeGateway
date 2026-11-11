namespace ThabeSoft.EdgeGateway.Lifecycles;


/// <summary>
/// 生命周期状态机
/// </summary>
public interface ILifecycleStateMachine : ILifecycleStateable
{
    ValueTask StartAsync(Func<CancellationToken, ValueTask> onStart, CancellationToken cancellationToken = default);
    ValueTask StopAsync(Func<CancellationToken, ValueTask> onStop, CancellationToken cancellationToken = default);
    ValueTask DisposeAsync(Func<ValueTask> onDispose);
}


/// <summary>
/// 可以多次连续启动和多次连续停止的状态机
/// </summary>
public interface IRetryableLifecycleStateMachine :
    ILifecycleStateMachine,
    IExecutor,
    IAsyncExecutor,
    IAsyncDisposable;


/// <summary>
/// 严格生命周期状态机, 不可连续重复启动, 不可连续重复停止
/// </summary>
public interface IStrictLifecycleStateMachine :
    ILifecycleStateMachine,
    IExecutor,
    IAsyncExecutor,
    IAsyncDisposable;