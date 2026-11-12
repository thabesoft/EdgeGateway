using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.ComponentModel;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;


internal sealed class ObserGateway(Gateway gateway) : IGateway, INotifyPropertyChanged, INotifyPropertyChanging
{



    public IReadOnlyDictionary<ConnectionEndpoint, IPort> Ports => throw new NotImplementedException();
    public LifecycleState State => throw new NotImplementedException();
    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;



    public async ValueTask<IPort> AddPortAsync(ITransportDescriptor transportDescriptor, CancellationToken cancellationToken = default)
    {
        var port = await gateway.AddPortAsync(transportDescriptor, cancellationToken);

    }

    public ValueTask<bool> RemovePortAsync(ConnectionEndpoint connectionEndpoint, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public sealed class Fucc<TKey, TValue>(ConcurrentDictionary<TKey, TValue> inner) : IDictionary<TKey, TValue>, INotifyCollectionChanged
{
    public TValue this[TKey key] { get => inner[key]; set => inner[key] = value; }
    public ICollection<TKey> Keys => inner.Keys;
    public ICollection<TValue> Values => inner.Values;
    public int Count => inner.Count;
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;
    public event NotifyCollectionChangedEventHandler? CollectionChanged;



    public void Add(TKey key, TValue value)
    {
        inner.TryAdd(key, value);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        throw new NotImplementedException();
    }

    public bool ContainsKey(TKey key)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public bool Remove(TKey key)
    {
        throw new NotImplementedException();
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        throw new NotImplementedException();
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}