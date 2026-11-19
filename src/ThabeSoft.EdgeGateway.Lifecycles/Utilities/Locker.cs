using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Lifecycles.Utilities;


internal sealed class Locker(SemaphoreSlim semaphoreSlim) : IDisposable
{
    private bool _innerLock;
    private readonly SemaphoreSlim _lock = semaphoreSlim;

    public Locker(int initSignal, int MaxSignal) : this(new SemaphoreSlim(initSignal, MaxSignal))
    {
        _innerLock = true;
    }

    public TimeSpan Timeout { get; init => field = value.Positive(); } = TimeSpan.FromSeconds(5);


    public async Task<IDisposable> WaitAsync(CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(Timeout);

        try
        {
            await _lock.WaitAsync(cts.Token);
            return new Releaser(() => _lock.Release());
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("获取锁已超时");
        }
    }

    public IDisposable Wait(CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(Timeout);

        try
        {
            _lock.Wait(cts.Token);
            return new Releaser(() => _lock.Release());
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("获取锁已超时");
        }
    }

    public void Dispose()
    {
        if (_innerLock) _lock.Dispose();
    }


    internal sealed class Releaser(Action action) : IDisposable
    {
        private Action? _action = action;
        public void Dispose() => Interlocked.Exchange(ref _action, null)?.Invoke();
    }
}