using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class ReadingSubject<T>(ILogger? logger = null) : IObservable<Reading<T>>, IObservable<Reading>, IDisposable
    where T : notnull
{
    private volatile int _disposed;
    private readonly ILogger _logger = logger ?? NullLogger.Instance;
    private readonly ConcurrentDictionary<Guid, IObserver<Reading<T>>> _observers = [];



    public IDisposable Subscribe(IObserver<Reading<T>> observer)
    {
        ObjectDisposedException.ThrowIf(_disposed != 0, this);

        var id = Guid.NewGuid();
        _observers[id] = observer;

        return Disposer.From(() => _observers.TryRemove(id, out _));
    }
    public IDisposable Subscribe(IObserver<Reading> observer)
    {
        return Subscribe(new Adapter(observer));
    }

    public void OnNext(Reading<T> value)
    {
        if (_disposed != 0) return;

        foreach (var observer in _observers.Values)
        {
            try
            {
                observer.OnNext(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                   "观察者处理数据时发生异常。数据类型: {DataType}, 当前值: {@Value}, 观察者类型: {ObserverType}",
                   typeof(T).Name,
                   value,
                   observer.GetType().Name);
            }
        }
    }
    public void OnError(Exception error)
    {
        if (_disposed != 0) return;

        foreach (var observer in _observers.Values)
        {
            try { observer.OnError(error); } catch { }
        }
    }
    public void OnCompleted()
    {
        if (_disposed != 0) return;

        foreach (var observer in _observers.Values)
        {
            try { observer.OnCompleted(); } catch { }
        }
    }


    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        var observers = _observers.Values;
        _observers.Clear();

        foreach (var observer in observers)
        {
            try { observer.OnCompleted(); } catch { }
        }
    }

    private sealed class Adapter(IObserver<Reading> observer) : IObserver<Reading<T>>
    {
        public void OnNext(Reading<T> value) => observer.OnNext(value);
        public void OnCompleted() => observer.OnCompleted();
        public void OnError(Exception error) => observer.OnError(error);
    }
}