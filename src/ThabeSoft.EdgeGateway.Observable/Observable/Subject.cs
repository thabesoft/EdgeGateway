namespace ThabeSoft.EdgeGateway.Observable;

internal sealed class Subject<T> : IObservable<T>
{
    private readonly List<IObserver<T>> _observers = new();
    private readonly object _gate = new();


    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (_gate)
        {
            _observers.Add(observer);
        }

        return new Subscription(this, observer);
    }

    public void Publish(T value)
    {
        IObserver<T>[] snapshot;
        lock (_gate)
        {
            snapshot = [.. _observers];
        }

        foreach (var observer in snapshot)
        {
            observer.OnNext(value);
        }
    }


    private void Unsubscribe(IObserver<T> observer)
    {
        lock (_gate)
        {
            _observers.Remove(observer);
        }
    }
    private sealed class Subscription(Subject<T> subject, IObserver<T> observer) : IDisposable
    {
        private int _disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1) return;
            subject.Unsubscribe(observer);
        }
    }
}