using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using ThabeSoft.EdgeGateway.Extensions;

namespace ThabeSoft.EdgeGateway.Lifecycles.Internal;


internal sealed class LifecycleBatchExecutor(ILogger? logger = null) : ILifecycleBatchExecutor
{
    private readonly ILogger _logger = logger ?? NullLogger.Instance;

#if DEBUG
    public TimeSpan ItemStartTimeout { get; init; } = TimeSpan.FromMinutes(5);
    public TimeSpan ItemStopTimeout { get; init; } = TimeSpan.FromMinutes(5);
    public TimeSpan ItemDisposeTimeout { get; init; } = TimeSpan.FromMinutes(5);
#else
    public TimeSpan ItemStartTimeout { get; init; } = TimeSpan.FromSeconds(30);
    public TimeSpan ItemStopTimeout { get; init; } = TimeSpan.FromSeconds(15);
    public TimeSpan ItemDisposeTimeout { get; init; } = TimeSpan.FromSeconds(10);
#endif


    [DebuggerStepThrough]
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
        throw new AggregateException(errors);
    }

    [DebuggerStepThrough]
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