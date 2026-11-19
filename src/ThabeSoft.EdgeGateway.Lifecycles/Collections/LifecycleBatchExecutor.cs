using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using ThabeSoft.EdgeGateway.Lifecycles.Utilities;

namespace ThabeSoft.EdgeGateway.Lifecycles.Collections;


internal sealed class LifecycleBatchExecutor(
        LifecycleBatchExecutorDescriptor descriptor,
        ILogger<LifecycleBatchExecutor> logger
    ) : ILifecycleBatchExecutor
{
    private readonly int _concurrency = descriptor.Concurrency;
    private readonly TimeSpan _itemStartTimeout = descriptor.ItemStartTimeout;
    private readonly TimeSpan _itemStopTimeout = descriptor.ItemStopTimeout;


    [DebuggerStepThrough]
    public async ValueTask StartBatchAsync(IEnumerable<ILifecycle> lifecycles, CancellationToken cancellationToken = default)
    {
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = _concurrency,
            CancellationToken = cancellationToken
        };

        var errors = new ConcurrentBag<Exception>();

        await Parallel.ForEachAsync(lifecycles, parallelOptions, async (lifecycle, ct) =>
        {
            if (lifecycle is null) return;

            try
            {
                await TimeoutExtensions.ExecuteAsync(lifecycle.StartAsync, _itemStartTimeout, "启动", ct);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                logger.LogError(ex, "{Type} 启动失败", lifecycle.GetType());

                try
                {
                    await TimeoutExtensions.ExecuteAsync(lifecycle.StopAsync, _itemStopTimeout, "停止");
                }
                catch (Exception stopEx)
                {
                    logger.LogCritical(stopEx, "{Type} 启动失败后，Stop 超时或者异常，资源泄漏风险", lifecycle.GetType());
                }
            }
        });

        if (errors.Count == 0) return;
        throw new AggregateException(errors);
    }

    [DebuggerStepThrough]
    public async ValueTask StopBatchAsync(IEnumerable<ILifecycle> lifecycles, CancellationToken cancellationToken)
    {
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = _concurrency,
            CancellationToken = cancellationToken
        };

        var errors = new ConcurrentBag<Exception>();

        await Parallel.ForEachAsync(lifecycles, parallelOptions, async (lifecycle, ct) =>
        {
            if (lifecycle is null) return;

            try
            {
                await TimeoutExtensions.ExecuteAsync(lifecycle.StopAsync, _itemStopTimeout, "停止", ct);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                logger.LogError(ex, "{Type} 停止失败", lifecycle.GetType());
            }
        });

        if (errors.Count == 0) return;
        throw new AggregateException(errors);
    }
}