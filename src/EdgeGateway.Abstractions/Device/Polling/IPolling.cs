using System.Collections.Concurrent;

namespace EdgeGateway.Device.Polling;


public interface IPollingTrigger : IAsyncDisposable
{
    bool IsRunning { get; }

    IDisposable Subscribe(Func<CancellationToken, ValueTask> onTick);

    ValueTask StartAsync(CancellationToken cancellationToken = default);
    ValueTask StopAsync(CancellationToken cancellationToken = default);
}

public sealed class FixedIntervalTrigger : IPollingTrigger
{
    private readonly System.Timers.Timer _timer;
    private CancellationTokenSource _cts = new();
    private readonly ConcurrentDictionary<Func<CancellationToken, ValueTask>, bool> _callbacks = [];


    public bool IsRunning { get; private set; }


    public FixedIntervalTrigger(TimeSpan interval)
    {
        _timer = new(interval.TotalMilliseconds);
        _timer.Elapsed += async delegate
        {
            foreach(var i in _callbacks)
            {
                await i.Key.Invoke(_cts.Token);
            }
        };
    }

    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        if (IsRunning) return ValueTask.CompletedTask;

        IsRunning = true;
        _timer.Start();

        return ValueTask.CompletedTask;
    }

    public ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        if (!IsRunning) return ValueTask.CompletedTask;

        IsRunning = false;
        _timer.Stop();

        return ValueTask.CompletedTask;
    }

    public IDisposable Subscribe(Func<CancellationToken, ValueTask> onTick)
    {
        _callbacks[onTick] = true;
        return new Disposer(() => _callbacks.TryRemove(onTick, out var _));
    }

    public ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _cts.Dispose();

        _timer.Stop();
        _timer.Dispose();

        return ValueTask.CompletedTask;
    }


    private class Disposer(Action disposeAction) : IDisposable
    {
        public void Dispose() => disposeAction.Invoke();
    }
}