using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;
using System.Diagnostics;
using ThabeSoft.EdgeGateway.Extensions;

namespace ThabeSoft.EdgeGateway.Lifecycles.Collections;


internal sealed class LifecycleBatchExecutor(ILogger? logger = null) : ILifecycleBatchExecutor
{
    private readonly ILogger _logger = logger ?? NullLogger.Instance;

#if DEBUG
    public int MaxCount { get; init; } = 10;
    public TimeSpan ItemStartTimeout { get; init; } = TimeSpan.FromMinutes(5);
    public TimeSpan ItemStopTimeout { get; init; } = TimeSpan.FromMinutes(5);
    public TimeSpan ItemDisposeTimeout { get; init; } = TimeSpan.FromMinutes(5);
#else
    public int MaxCount { get; init; } = 10;
    public TimeSpan ItemStartTimeout { get; init; } = TimeSpan.FromSeconds(30);
    public TimeSpan ItemStopTimeout { get; init; } = TimeSpan.FromSeconds(15);
    public TimeSpan ItemDisposeTimeout { get; init; } = TimeSpan.FromSeconds(10);
#endif


    [DebuggerStepThrough]
    public async ValueTask StartAsync(IEnumerable<ILifecycle> lifecycles, CancellationToken cancellationToken = default)
    {
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = MaxCount,
            CancellationToken = cancellationToken
        };

        var errors = new ConcurrentBag<Exception>();

        await Parallel.ForEachAsync(lifecycles, parallelOptions, async (lifecycle, ct) =>
        {
            if (lifecycle is null) return;

            try
            {
                await TimeoutExtensions.ExecuteAsync(lifecycle.StartAsync, ItemStartTimeout, "启动", ct);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                _logger.LogError(ex, "{Type} 启动失败", lifecycle.GetType());

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
            MaxDegreeOfParallelism = MaxCount,
            CancellationToken = cancellationToken
        };

        var errors = new ConcurrentBag<Exception>();

        await Parallel.ForEachAsync(lifecycles, parallelOptions, async (lifecycle, ct) =>
        {
            if (lifecycle is null) return;

            try
            {
                await TimeoutExtensions.ExecuteAsync(lifecycle.StopAsync, ItemStartTimeout, "停止", cancellationToken);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                _logger.LogError(ex, "{Type} 停止失败", lifecycle.GetType());
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
                await TimeoutExtensions.ExecuteAsync(lifecycle.DisposeAsync, ItemStartTimeout, "释放");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Type} 释放失败", lifecycle.GetType());
            }
        }
    }
}