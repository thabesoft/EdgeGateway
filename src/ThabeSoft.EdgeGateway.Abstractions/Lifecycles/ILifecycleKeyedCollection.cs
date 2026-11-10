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