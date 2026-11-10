using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ThabeSoft.EdgeGateway.Extensions;

namespace ThabeSoft.EdgeGateway.Lifecycles;


internal sealed class LifecycleBatchExecutor(ILogger? logger = null) : ILifecycleBatchExecutor
{
    private readonly ILogger _logger = logger ?? NullLogger.Instance;

    public TimeSpan ItemStartTimeout { get; init; } = TimeSpan.FromSeconds(30);
    public TimeSpan ItemStopTimeout { get; init; } = TimeSpan.FromSeconds(15);
    public TimeSpan ItemDisposeTimeout { get; init; } = TimeSpan.FromSeconds(10);


    public async ValueTask StartAsync(IEnumerable<ILifecycle> lifecycles, CancellationToken cancellationToken = default)
    {
        List<Exception> errors = [];

        foreach (var lifecycle in lifecycles)
        {
            if (lifecycle is null) continue;

            try
            {
                await TimeoutExtensions.ExecuteAsync(lifecycle.StartAsync, ItemStartTimeout, "启动", cancellationToken);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                _logger.LogError(ex, "{Type} 启动失败", lifecycle.GetType());
            }
        }

        if (errors.Count == 0) return;
        throw new AggregateException($"部分启动失败, 共 {errors.Count} 个", errors);
    }
    public async ValueTask StopAsync(IEnumerable<ILifecycle> lifecycles, CancellationToken cancellationToken)
    {
        List<Exception> errors = [];

        foreach (var lifecycle in lifecycles)
        {
            if (lifecycle is null) continue;

            try
            {
                await TimeoutExtensions.ExecuteAsync(lifecycle.StopAsync, ItemStartTimeout, "停止", cancellationToken);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                _logger.LogError(ex, "{Type} 停止失败", lifecycle.GetType());
            }
        }

        if (errors.Count == 0) return;
        throw new AggregateException($"部分停止失败, 共 {errors.Count} 个", errors);
    }
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