using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EdgeGateway;


public interface ILifecycle
{
    bool IsRunning { get; }

    ValueTask StartAsync(CancellationToken cancellationToken = default);
    ValueTask StopAsync(CancellationToken cancellationToken = default);
}


public abstract class Lifecycle(ILogger? logger = null) : ILifecycle, IAsyncDisposable
{
    private int _isRunning;
    private int _disposed;
    private readonly ILogger _logger = logger ?? NullLogger.Instance;


    protected bool IsDisposed => Volatile.Read(ref _disposed) == 1;
    public bool IsRunning => Volatile.Read(ref _isRunning) == 1;


    public virtual async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        if (Interlocked.CompareExchange(ref _isRunning, 1, 0) != 0)
            return;

        try
        {
            await OnStartAsync(cancellationToken);
        }
        catch
        {
            Interlocked.Exchange(ref _isRunning, 0);
            throw;
        }
    }

    public virtual async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        if (Interlocked.CompareExchange(ref _isRunning, 0, 1) != 1)
            return;

        await OnStopAsync(cancellationToken);
    }

    public virtual async ValueTask DisposeAsync()
    {
        if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
            return;

        try { await StopAsync(CancellationToken.None); }
        catch (Exception ex) { _logger.LogError(ex, "停止时发生错误"); }

        try { await OnDisposeAsync(); }
        catch (Exception ex) { _logger.LogError(ex, "释放时发生错误"); }

        GC.SuppressFinalize(this);
    }


    protected abstract ValueTask OnStartAsync(CancellationToken cancellationToken);
    protected abstract ValueTask OnStopAsync(CancellationToken cancellationToken);
    protected virtual ValueTask OnDisposeAsync() => ValueTask.CompletedTask;
}