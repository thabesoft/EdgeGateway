namespace ThabeSoft.EdgeGateway.Lifecycles.Collections;


/// <summary>
/// 带有 键 的生命周期集合
/// </summary>
public interface ILifecycleKeyedCollection<TKey, TLifecycle> : IAsyncDisposable
    where TKey : notnull
    where TLifecycle : ILifecycle
{
    IReadOnlyDictionary<TKey, TLifecycle> Items { get; }

    ValueTask<TLifecycle> AddAsync(TKey key, Func<TLifecycle> getValueHandler, CancellationToken cancellationToken = default);
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
