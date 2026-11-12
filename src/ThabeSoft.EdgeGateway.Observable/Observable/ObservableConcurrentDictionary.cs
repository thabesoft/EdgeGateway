using System.Collections;
using System.Collections.Concurrent;

namespace ThabeSoft.EdgeGateway.Observable;


internal sealed class ObservableConcurrentDictionary<TKey, TValue> : IObservableDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, TValue> _inner = [];
    private Subject<DictionaryAdded<TKey, TValue>> _added = new();
    private Subject<DictionaryRemoved<TKey, TValue>> _removed = new();
    private Subject<DictionaryReplaced<TKey, TValue>> _replaced = new();
    private Subject<DictionaryReset> _reset = new();
    private Subject<DictionaryCountChanged> _countChanged = new();


    public int Count => _inner.Count;
    public IEnumerable<TKey> Keys => _inner.Keys;
    public IEnumerable<TValue> Values => _inner.Values;


    public IObservable<DictionaryAdded<TKey, TValue>> Added => _added;
    public IObservable<DictionaryRemoved<TKey, TValue>> Removed => _removed;
    public IObservable<DictionaryReplaced<TKey, TValue>> Replaced => _replaced;
    public IObservable<DictionaryReset> Reset => _reset;
    public IObservable<DictionaryCountChanged> CountChanged => _countChanged;



    public TValue this[TKey key]
    {
        get => _inner[key];
        set
        {
            if (_inner.TryGetValue(key, out var oldValue))
            {
                if (EqualityComparer<TValue>.Default.Equals(oldValue, value))
                    return;

                _inner[key] = value;
                OnCollectionReplaced(key, value, oldValue);
            }
            else
            {
                int oldCount = _inner.Count;
                _inner[key] = value;

                OnCountChanged(Count, oldCount);
                OnCollectionAdded(key, value);
            }
        }
    }
    public bool TryAdd(TKey key, TValue value)
    {
        int oldCount = _inner.Count;
        if (!_inner.TryAdd(key, value))
            return false;

        OnCountChanged(Count, oldCount);
        OnCollectionAdded(key, value);

        return true;
    }
    public bool TryRemove(TKey key, out TValue value)
    {
        int oldCount = _inner.Count;
        if (!_inner.TryRemove(key, out value))
            return false;

        OnCountChanged(Count, oldCount);
        OnCollectionRemoved(key, value);

        return true;
    }
    public void Clear()
    {
        int oldCount = _inner.Count;
        _inner.Clear();

        OnCountChanged(0, oldCount);
        OnCollectionReseted();
    }



    public bool ContainsKey(TKey key) => _inner.ContainsKey(key);
    public bool TryGetValue(TKey key, out TValue value) => _inner.TryGetValue(key, out value);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _inner.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();



    private void OnCollectionAdded(TKey key, TValue value)
    {
        _added.Publish(new(key, value));
    }
    private void OnCollectionRemoved(TKey key, TValue value)
    {
        _removed.Publish(new(key, value));
    }
    private void OnCollectionReplaced(TKey key, TValue newValue, TValue oldValue)
    {
        _replaced.Publish(new(key, oldValue, newValue));
    }
    private void OnCollectionReseted()
    {
        _reset.Publish(new());
    }
    private void OnCountChanged(int newValue, int oldValue)
    {
        _countChanged.Publish(new(oldValue, newValue));
    }
}