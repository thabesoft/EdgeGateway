using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;

namespace ThabeSoft.EdgeGateway.Lifecycles;


/// <summary>
/// 带有 键 的生命周期集合
/// </summary>
public interface ILifecycleKeyedCollection<TKey, TLifecycle>
    where TKey : notnull
    where TLifecycle : ILifecycle, IAsyncDisposable
{
    IReadOnlyDictionary<TKey, TLifecycle> Items { get; }

    ValueTask<TLifecycle> AddAsync(TKey key, Func<TLifecycle> getValueHandler, CancellationToken cancellationToken = default);
    ValueTask<bool> RemoveAsync(TKey key, CancellationToken cancellationToken = default);
}

public static class LifecycleKeyedCollectionExtensions
{
    extension<TKey, TLifecycle>(ILifecycleKeyedCollection<TKey, TLifecycle> collection)
        where TKey : notnull
        where TLifecycle : ILifecycle, IAsyncDisposable
    {
        public IEnumerable<TKey> Keys => collection.Items.Keys;
        public IEnumerable<TLifecycle> Values => collection.Items.Values;
    }
}


internal sealed class LifecycleKeyedCollection<TKey, TLifecycle> : ILifecycleKeyedCollection<TKey, TLifecycle>
    where TKey : notnull
    where TLifecycle : ILifecycle, IAsyncDisposable
{
    private readonly ConcurrentDictionary<TKey, TLifecycle> _items = [];
    private readonly ILifecycleStateable _stateSource;
    private readonly ILogger _logger;

    public LifecycleKeyedCollection(
            ILifecycleStateable stateSource,
            ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(stateSource);

        _stateSource = stateSource;
        _logger = logger ?? NullLogger.Instance;
    }

    public IReadOnlyDictionary<TKey, TLifecycle> Items => _items;

    public async ValueTask<TLifecycle> AddAsync(TKey key, Func<TLifecycle> getValueHandler, CancellationToken cancellationToken = default)
    {
        if (_items.ContainsKey(key))
            throw new InvalidOperationException($"{key} 已存在, 无法添加");

        var item = getValueHandler();

        if (!_items.TryAdd(key, item))
        {
            try { await item.DisposeAsync(); } catch { }
            throw new InvalidOperationException($"{key} 已存在");
        }

        if (_stateSource.State == LifecycleState.Running)
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
        catch (Exception ex) { _logger.LogError(ex, "移除 [{Type}]  时发生错误, 停止失败", item.GetType()); }

        try { await item.DisposeAsync(); }
        catch (Exception ex) { _logger.LogError(ex, "移除 [{Type}] 时发生错误, 释放失败", key); }

        return true;
    }
}
