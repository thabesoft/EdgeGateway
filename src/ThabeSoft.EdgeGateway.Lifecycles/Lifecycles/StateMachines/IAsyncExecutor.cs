namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines;


/// <summary>
/// 异步执行器
/// </summary>
public interface IAsyncExecutor
{
    ValueTask ExecuteAsync(
        Func<CancellationToken, ValueTask> method,
        CancellationToken cancellationToken = default);

    ValueTask<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, ValueTask<TResult>> method,
        CancellationToken cancellationToken = default);
}