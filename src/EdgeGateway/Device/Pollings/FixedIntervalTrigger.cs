using EdgeGateway.Device.Polling;
using System.Collections.Concurrent;

namespace EdgeGateway.Device.Pollings;


public sealed class FixedIntervalTrigger : Lifecycle, IPollingTrigger
{
    private readonly System.Timers.Timer _timer;
    private CancellationTokenSource _cts = new();
    private int _elapsedRunning;
    private readonly ConcurrentDictionary<Func<CancellationToken, ValueTask>, bool> _callbacks = [];


    public FixedIntervalTrigger(TimeSpan interval)
    {
        _timer = new(interval.TotalMilliseconds);
        _timer.Elapsed += async delegate
        {
            if (Interlocked.Exchange(ref _elapsedRunning, 1) == 1) return;

            try
            {
                foreach (var i in _callbacks)
                {
                    try { await i.Key.Invoke(_cts.Token); }
                    catch (OperationCanceledException) { /* 停止时取消，预期 */ }
                    catch (Exception) { /* 记日志，继续下一个回调 */ }
                }
            }
            finally { Interlocked.Exchange(ref _elapsedRunning, 0); }
        };
    }

    public IDisposable Subscribe(Func<CancellationToken, ValueTask> onTick)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        _callbacks[onTick] = true;
        return new Disposer(() => _callbacks.TryRemove(onTick, out var _));
    }


    protected override ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        _cts.Dispose();
        _cts = new CancellationTokenSource();
        _timer.Start();

        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        _timer.Stop();
        _cts.Cancel();

        return ValueTask.CompletedTask;
    }

    protected override async ValueTask OnDisposeAsync()
    {
        _cts.Dispose();
        _timer.Dispose();
    }

    private class Disposer(Action disposeAction) : IDisposable
    {
        public void Dispose() => disposeAction.Invoke();
    }
}