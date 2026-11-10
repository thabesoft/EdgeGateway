namespace ThabeSoft.EdgeGateway.Lifecycles;

/// <summary>
/// 生命中周期批量处理器
/// </summary>
public interface ILifecycleBatchExecutor
{
    ValueTask StartAsync(IEnumerable<ILifecycle> lifecycles, CancellationToken cancellationToken = default);
    ValueTask StopAsync(IEnumerable<ILifecycle> lifecycles, CancellationToken cancellationToken = default);
    ValueTask DisposeAsync<TLifecycle>(IEnumerable<TLifecycle> lifecycles) where TLifecycle : ILifecycle, IAsyncDisposable;
}