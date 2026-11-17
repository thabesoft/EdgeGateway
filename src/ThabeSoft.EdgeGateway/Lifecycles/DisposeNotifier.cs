using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Lifecycles;


internal sealed class DisposeNotifier(ILogger? logger = null) : IDisposeObservable, IAsyncDisposable
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly ILogger _logger = logger ?? NullLogger.Instance;
    private readonly List<Func<ValueTask>> _handlers = [];
    private bool _isDisposed;


    public IDisposable Subscribe(Func<ValueTask> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        _lock.Wait();
        try
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            // 👈 利用 List.Contains 结合委托的 Equals 实现去重
            if (!_handlers.Contains(handler))
            {
                _handlers.Add(handler);
            }
        }
        finally { _lock.Release(); }

        return Disposer.From(() =>
        {
            _lock.Wait();
            try { _handlers.Remove(handler); }
            finally { _lock.Release(); }
        });
    }

    public async ValueTask RaiseAsync<T>(T obj)
    {
        Func<ValueTask>[] handlersToInvoke;

        await _lock.WaitAsync();
        try
        {
            if (_isDisposed || _handlers.Count == 0) return;

            handlersToInvoke = [.. _handlers];
            _handlers.Clear();
        }
        finally { _lock.Release(); }

        foreach (var handler in handlersToInvoke)
        {
            try
            {
                await handler.Invoke();
            }
            catch (Exception ex)
            {
                var context = obj switch
                {
                    null => "未知对象",
                    string str => str,
                    _ => obj.GetType().ToString()
                };

                _logger.LogError(ex, "[{Context}] 释放事件异步回调异常", context);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _lock.WaitAsync();
        try
        {
            if (_isDisposed) return;
            _isDisposed = true;
        }
        finally
        {
            _lock.Release();
        }

        await RaiseAsync(nameof(DisposeNotifier));

        // 清空
        await _lock.WaitAsync();
        try
        {
            _handlers.Clear();
        }
        finally
        {
            _lock.Release();
        }

    }
}