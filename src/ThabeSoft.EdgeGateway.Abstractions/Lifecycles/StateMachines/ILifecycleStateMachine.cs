namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines;


/// <summary>
/// 生命周期状态机
/// </summary>
public interface ILifecycleStateMachine : ILifecycleStateable, IExecutor, IAsyncExecutor, ILifecycleStateNotifier
{
    ValueTask TransitionToRunningAsync(Func<CancellationToken, ValueTask> onStart, CancellationToken cancellationToken = default);
    ValueTask TransitionToStoppedAsync(Func<CancellationToken, ValueTask> onStop, CancellationToken cancellationToken = default);
}