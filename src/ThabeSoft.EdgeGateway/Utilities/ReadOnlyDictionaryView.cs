using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ThabeSoft.EdgeGateway.Utilities;


internal sealed class ReadOnlyDictionaryView<TKey, TValue, TView>(IReadOnlyDictionary<TKey, TValue> items) : IReadOnlyDictionary<TKey, TView>
    where TKey : notnull
    where TValue : TView
{
    public IEnumerable<TKey> Keys => items.Keys;
    public IEnumerable<TView> Values
    {
        get { foreach (var i in items.Values) yield return i; }
    }
    public int Count => items.Count;
    public TView this[TKey key] => items[key];


    public bool ContainsKey(TKey key) => items.ContainsKey(key);
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TView view)
    {
        var resutl = items.TryGetValue(key, out var value);
        view = value;

        return resutl;
    }

    public IEnumerator<KeyValuePair<TKey, TView>> GetEnumerator()
    {
        foreach (var kv in items)
            yield return new KeyValuePair<TKey, TView>(kv.Key, kv.Value);
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}