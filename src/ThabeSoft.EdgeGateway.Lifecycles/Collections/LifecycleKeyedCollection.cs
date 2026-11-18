using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace ThabeSoft.EdgeGateway.Lifecycles.Collections;


internal sealed class LifecycleKeyedCollection<TKey, TLifecycle>(
        ILifecycleStateable stateSource,
        ILogger<LifecycleKeyedCollection<TKey, TLifecycle>> logger
    ) : ILifecycleKeyedCollection<TKey, TLifecycle>
    where TKey : notnull
    where TLifecycle : ILifecycle, IAsyncDisposable
{
    private readonly ConcurrentDictionary<TKey, TLifecycle> _items = [];


    public IReadOnlyDictionary<TKey, TLifecycle> Items => _items;

    public async ValueTask<TLifecycle> AddAsync(TKey key, Func<TLifecycle> getValueHandler, CancellationToken cancellationToken = default)
    {
        var item = getValueHandler();
        if (!_items.TryAdd(key, item))
        {
            await item.DisposeAsync();
            throw new InvalidOperationException($"{key} 已存在, 无法添加");
        }

        if (stateSource.State == LifecycleState.Running)
        {
            try
            {
                await item.StartAsync(cancellationToken);
            }
            catch
            {
                _items.TryRemove(key, out _);
                try { await item.DisposeAsync(); } catch { }

                throw;
            }
        }

        return item;
    }
    public async ValueTask<bool> RemoveAsync(TKey key, CancellationToken cancellationToken = default)
    {
        if (!_items.TryRemove(key, out var item))
            return false;

        try { await item.StopAsync(cancellationToken); }
        catch (Exception ex) { logger.LogError(ex, "移除 [{Type}]  时发生错误, 停止失败", item.GetType()); }

        try { await item.DisposeAsync(); }
        catch (Exception ex) { logger.LogError(ex, "移除 [{Type}] 时发生错误, 释放失败", key); }

        return true;
    }
    public async ValueTask DisposeAsync()
    {
        foreach (var i in _items.Values)
        {
            try
            {
                await i.DisposeAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "生命周期集合释放时发生错误");
            }
        }
    }
}