namespace ThabeSoft.EdgeGateway.Lifecycles.Collections;


/// <summary>
/// 带有 键 的生命周期集合
/// </summary>
public interface ILifecycleKeyedCollection<TKey, TLifecycle> : IAsyncDisposable
    where TKey : notnull
    where TLifecycle : ILifecycle
{
    IReadOnlyDictionary<TKey, TLifecycle> Items { get; }

    /// <summary>
    /// 添加一个生命周期对象, 集合接管该对象
    /// </summary>
    ValueTask<TLifecycle> AddAsync(TKey key, Func<TLifecycle> lifecycleProvider, CancellationToken cancellationToken = default);

    /// <summary>
    /// 从集合中移除并停止和释放生命周期
    /// </summary>
    ValueTask<bool> RemoveAsync(TKey key, CancellationToken cancellationToken = default);
}

/// <summary>
/// 带有 键 的生命周期集合
/// </summary>
public interface ILifecycleKeyedCollection<TKey> : ILifecycleKeyedCollection<TKey, ILifecycle>
    where TKey : notnull;


public static class LifecycleKeyedCollectionExtensions
{
    extension<TKey, TLifecycle>(ILifecycleKeyedCollection<TKey, TLifecycle> collection)
        where TKey : notnull
        where TLifecycle : ILifecycle
    {
        public IEnumerable<TKey> Keys => collection.Items.Keys;
        public IEnumerable<TLifecycle> Values => collection.Items.Values;
    }
}
