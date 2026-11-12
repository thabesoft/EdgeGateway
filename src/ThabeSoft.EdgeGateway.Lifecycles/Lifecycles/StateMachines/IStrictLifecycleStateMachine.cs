namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines;

/// <summary>
/// 严格生命周期状态机, 不可连续重复启动, 不可连续重复停止
/// </summary>
public interface IStrictLifecycleStateMachine :
    ILifecycleStateMachine,
    IExecutor,
    IAsyncExecutor,
    IAsyncDisposable;