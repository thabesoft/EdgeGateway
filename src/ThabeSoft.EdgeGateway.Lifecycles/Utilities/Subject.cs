namespace ThabeSoft.EdgeGateway.Utilities;


internal sealed class Subject<T> : IObservable<T>, IObserver<T>, IDisposable
{
    private readonly object _sync = new();
    private readonly List<IObserver<T>> _observers = [];

    private bool _stopped;
    private bool _disposed;
    private Exception? _error;

    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (_sync)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (!_stopped)
            {
                _observers.Add(observer);
                return new Subscription(this, observer);
            }
        }

        // 已经终止，向新订阅者立即发送终止通知。
        if (_error is { } error)
            observer.OnError(error);
        else
            observer.OnCompleted();

        return EmptyDisposable.Instance;
    }

    public void OnNext(T value)
    {
        IObserver<T>[] observers;

        lock (_sync)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (_stopped)
                return;

            observers = [.. _observers];
        }

        foreach (var observer in observers)
            observer.OnNext(value);
    }

    public void OnError(Exception error)
    {
        ArgumentNullException.ThrowIfNull(error);

        IObserver<T>[] observers;

        lock (_sync)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (_stopped)
                return;

            _stopped = true;
            _error = error;

            observers = [.. _observers];
            _observers.Clear();
        }

        foreach (var observer in observers)
            observer.OnError(error);
    }

    public void OnCompleted()
    {
        IObserver<T>[] observers;

        lock (_sync)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (_stopped)
                return;

            _stopped = true;

            observers = [.. _observers];
            _observers.Clear();
        }

        foreach (var observer in observers)
            observer.OnCompleted();
    }

    public void Dispose()
    {
        lock (_sync)
        {
            if (_disposed)
                return;

            _disposed = true;
            _observers.Clear();
        }
    }

    private void Unsubscribe(IObserver<T> observer)
    {
        lock (_sync)
        {
            _observers.Remove(observer);
        }
    }

    private sealed class Subscription(
        Subject<T> subject,
        IObserver<T> observer) : IDisposable
    {
        private Subject<T>? _subject = subject;

        public void Dispose()
        {
            Interlocked.Exchange(ref _subject, null)?
                .Unsubscribe(observer);
        }
    }

    private sealed class EmptyDisposable : IDisposable
    {
        public static readonly EmptyDisposable Instance = new();

        public void Dispose() { }
    }
}
