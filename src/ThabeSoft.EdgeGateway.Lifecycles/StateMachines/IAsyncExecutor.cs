namespace ThabeSoft.EdgeGateway.StateMachines;


/// <summary>
/// 异步执行器
/// </summary>
public interface IAsyncExecutor
{
    ValueTask ExecuteAsync(
        Func<CancellationToken, ValueTask> action,
        CancellationToken cancellationToken = default);

    ValueTask<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, ValueTask<TResult>> action,
        CancellationToken cancellationToken = default);
}