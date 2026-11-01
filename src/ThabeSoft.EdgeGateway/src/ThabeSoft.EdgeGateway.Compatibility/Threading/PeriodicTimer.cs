#if NETSTANDARD2_0

namespace System.Threading;

public sealed class PeriodicTimer : IDisposable
{
    private readonly TimeProvider _timeProvider;
    private readonly CancellationTokenSource _disposeCts = new();
    private readonly object _gate = new();

    private ITimer? _timer;
    private TaskCompletionSource<bool>? _pendingTick;
    private bool _disposed;
    private TimeSpan _period;


    public PeriodicTimer(TimeSpan period, TimeProvider timeProvider)
    {
        if (period <= TimeSpan.Zero && period != Timeout.InfiniteTimeSpan)
            throw new ArgumentOutOfRangeException(nameof(period));

        ArgumentNullException.ThrowIfNull(timeProvider);


        Period = period;
        _timeProvider = timeProvider;
    }

    public PeriodicTimer(TimeSpan period) : this(period, TimeProvider.System) { }


    public TimeSpan Period
    {
        get { lock (_gate) return _period; }
        set
        {
            if (value <= TimeSpan.Zero && value != Timeout.InfiniteTimeSpan)
                throw new ArgumentOutOfRangeException(nameof(value));
            lock (_gate)
            {
                _period = value;
                // 已启动的定时器改周期：下一次到期按新周期算
                _timer?.Change(value, Timeout.InfiniteTimeSpan);
            }
        }
    }

    public ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            // 同一时刻只允许一个等待者（与内置 PeriodicTimer 语义一致）
            if (_pendingTick is not null)
                throw new InvalidOperationException("WaitForNextTickAsync 不允许并发调用。");

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _pendingTick = tcs;

            // 首次等待时启动定时器；周期由 Period 决定
            _timer ??= _timeProvider.CreateTimer(
                static state => ((PeriodicTimer)state!).OnTimer(),
                this,
                _period,
                Timeout.InfiniteTimeSpan); // 单次触发，每次 Tick 后重新 Change

            if (cancellationToken.CanBeCanceled)
            {
                var registration = cancellationToken.Register(s =>
                {
                    var (self, t) = ((PeriodicTimer, TaskCompletionSource<bool>))s!;
                    lock (self._gate)
                    {
                        if (ReferenceEquals(self._pendingTick, t))
                            self._pendingTick = null;
                    }
                    t.TrySetCanceled(cancellationToken);
                }, (this, tcs));
                tcs.Task.ContinueWith(_ => registration.Dispose(), TaskScheduler.Default);
            }

            return new ValueTask<bool>(tcs.Task);
        }
    }

    private void OnTimer()
    {
        TaskCompletionSource<bool>? tcs;
        lock (_gate)
        {
            if (_disposed) return;
            tcs = _pendingTick;
            _pendingTick = null;
            // 重新排下一次到期，周期用当前 Period
            _timer?.Change(_period, Timeout.InfiniteTimeSpan);
        }
        tcs?.TrySetResult(true);
    }

    public void Dispose()
    {
        TaskCompletionSource<bool>? tcs;
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
            _timer?.Dispose();
            _timer = null;
            tcs = _pendingTick;
            _pendingTick = null;
        }
        tcs?.TrySetResult(false); // 与内置行为一致：Dispose 时等待者得到 false
        _disposeCts.Cancel();
        _disposeCts.Dispose();
    }
}

#endif