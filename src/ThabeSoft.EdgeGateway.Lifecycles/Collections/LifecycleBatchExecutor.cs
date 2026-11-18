using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Collections;


internal sealed class LifecycleBatchExecutor(LifecycleBatchExecutorDescriptor descriptor, ILogger<LifecycleBatchExecutor> logger) : ILifecycleBatchExecutor
{
    private readonly int _concurrency = descriptor.Concurrency;
    private readonly TimeSpan _itemStartTimeout = descriptor.ItemStartTimeout;
    private readonly TimeSpan _itemStopTimeout = descriptor.ItemStopTimeout;
    private readonly TimeSpan _itemDisposeTimeout = descriptor.ItemDisposeTimeout;


    [DebuggerStepThrough]
    public async ValueTask StartAsync(IEnumerable<ILifecycle> lifecycles, CancellationToken cancellationToken = default)
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

                try { await lifecycle.StopAsync(ct); } catch { }
            }
        });

        if (errors.Count == 0) return;
        throw new AggregateException(errors);
    }

    [DebuggerStepThrough]
    public async ValueTask StopAsync(IEnumerable<ILifecycle> lifecycles, CancellationToken cancellationToken)
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
                await TimeoutExtensions.ExecuteAsync(lifecycle.StopAsync, _itemStopTimeout, "停止", cancellationToken);
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

    [DebuggerStepThrough]
    public async ValueTask DisposeAsync<TLifecycle>(IEnumerable<TLifecycle> lifecycles) where TLifecycle : ILifecycle, IAsyncDisposable
    {
        foreach (var lifecycle in lifecycles)
        {
            try
            {
                await TimeoutExtensions.ExecuteAsync(lifecycle.DisposeAsync, _itemDisposeTimeout, "释放");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Type} 释放失败", lifecycle.GetType());
            }
        }
    }
}