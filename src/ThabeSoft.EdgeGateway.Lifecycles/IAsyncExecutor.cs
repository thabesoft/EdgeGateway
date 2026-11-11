namespace ThabeSoft.EdgeGateway.Lifecycles;


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


public static class AsyncExecutorExtensions
{
    extension<TResult>(Func<CancellationToken, ValueTask<TResult>> method)
    {
        public Func<CancellationToken, ValueTask<TResult>> WithExecutor(IAsyncExecutor executor)
        {
            return ct => executor.ExecuteAsync(ct => method(ct), ct);
        }
    }
}