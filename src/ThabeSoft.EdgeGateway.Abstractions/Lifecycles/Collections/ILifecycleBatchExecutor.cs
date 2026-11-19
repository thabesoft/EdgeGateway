namespace ThabeSoft.EdgeGateway.Lifecycles.Collections;


/// <summary>
/// 生命中周期批量处理器
/// </summary>
public interface ILifecycleBatchExecutor
{
    ValueTask StartBatchAsync(IEnumerable<ILifecycle> lifecycles, CancellationToken cancellationToken = default);
    ValueTask StopBatchAsync(IEnumerable<ILifecycle> lifecycles, CancellationToken cancellationToken = default);
    ValueTask DisposeBatchAsync(IEnumerable<ILifecycle> lifecycles);
}