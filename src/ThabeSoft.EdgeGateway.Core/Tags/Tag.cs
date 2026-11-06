using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Addressing;

namespace ThabeSoft.EdgeGateway.Tags;


internal sealed record class Tag<T>(TagDescriptor<T> tag) : ITag<T>
    where T : notnull
{
    public TagId Id => tag.Id;
    public IDataAddress Address => tag.Address;
    object? ITag.Value => Value;


    public DateTimeOffset Timestamp { get; private set; }
    public TagQuality Quality { get; private set; }
    public T? Value { get; private set; }


    public void Update(T value)
    {
        foreach (var i in ts.Keys)
            i.Invoke(value);

        foreach (var i in os.Keys)
            i.Invoke(value);
    }

    public IDisposable Subscribe(Action<T> callback)
    {
        ts[callback] = default;
        return new Disposer(() => ts.TryRemove(callback, out _));
    }
    public IDisposable Subscribe(Action<object> callback)
    {
        os[callback] = default;
        return new Disposer(() => os.TryRemove(callback, out _));
    }


    private ConcurrentDictionary<Action<T>, byte> ts = [];
    private ConcurrentDictionary<Action<object>, byte> os = [];

    private sealed class Disposer(Action action) : IDisposable { void IDisposable.Dispose() => action.Invoke(); }
}