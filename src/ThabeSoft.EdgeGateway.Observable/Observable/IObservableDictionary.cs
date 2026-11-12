namespace ThabeSoft.EdgeGateway.Observable;


public interface IObservableDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    IObservable<DictionaryCountChanged> CountChanged { get; }
    IObservable<DictionaryAdded<TKey, TValue>> Added { get; }
    IObservable<DictionaryRemoved<TKey, TValue>> Removed { get; }
    IObservable<DictionaryReplaced<TKey, TValue>> Replaced { get; }
    IObservable<DictionaryReset> Reset { get; }


    new TValue this[TKey key] { get; set; }
    bool TryAdd(TKey key, TValue value);
    bool TryRemove(TKey key, out TValue value);
    void Clear();
}

public readonly record struct DictionaryAdded<TKey, TValue>(TKey Key, TValue Value) where TKey : notnull;
public readonly record struct DictionaryRemoved<TKey, TValue>(TKey Key, TValue Value) where TKey : notnull;
public readonly record struct DictionaryReplaced<TKey, TValue>(TKey Key, TValue OldValue, TValue NewValue) where TKey : notnull;
public readonly record struct DictionaryReset;
public readonly record struct DictionaryCountChanged(int OldValue, int NewValue);
