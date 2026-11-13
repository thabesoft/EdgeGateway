namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines;

/// <summary>
/// 可以多次连续启动和多次连续停止的状态机
/// </summary>
public interface IRetryableLifecycleStateMachine :
    ILifecycleStateMachine,
    IExecutor,
    IAsyncExecutor,
    IAsyncDisposable;