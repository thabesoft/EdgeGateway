namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines;


/// <summary>
/// 生命周期状态机
/// </summary>
public interface ILifecycleStateMachine : ILifecycleStateable, IExecutor, IAsyncExecutor, IAsyncDisposable
{
    ValueTask StartAsync(Func<CancellationToken, ValueTask> onStart, CancellationToken cancellationToken = default);
    ValueTask StopAsync(Func<CancellationToken, ValueTask> onStop, CancellationToken cancellationToken = default);
    ValueTask DisposeAsync(Func<ValueTask> onDispose);
}