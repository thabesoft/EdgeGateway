using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ThabeSoft.EdgeGateway.Lifecycles.Internal;


internal sealed class BackgroundLifecycle : IBackgroundLifecycle
{
    private int _disposed;
    private readonly ILogger _logger;


    private readonly Func<CancellationToken, Task> _backgroundAction;
    private Task? _backgroundTask;
    private CancellationTokenSource? _backgroundTaskCts;


    public BackgroundLifecycle(Func<CancellationToken, Task> backgroundAction, ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(backgroundAction);

        _backgroundAction = backgroundAction;
        _logger = logger ?? NullLogger.Instance;
    }


    public TimeSpan DisposeTimeout { get; init; } = TimeSpan.FromSeconds(10);


    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        if (_backgroundTask is { IsCompleted: false })
            return ValueTask.CompletedTask;

        _backgroundTaskCts = new CancellationTokenSource();
        _backgroundTask = _backgroundAction.Invoke(_backgroundTaskCts.Token);
        return ValueTask.CompletedTask;
    }
    public async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        if (_backgroundTaskCts is null || _backgroundTask is null)
            return;

        await _backgroundTaskCts.CancelAsync();

        try { await _backgroundTask; }
        catch (OperationCanceledException) { }
        catch (Exception ex) { _logger.LogError(ex, "触发器任务停止失败"); }

        _backgroundTaskCts.Dispose();
        _backgroundTaskCts = null;
        _backgroundTask = null;
    }
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1) return;

        try
        {
            await TimeoutExtensions.ExecuteAsync(StopAsync, DisposeTimeout, "释放");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "释放时发生错误");
        }
    }
}