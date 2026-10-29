using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;

namespace EdgeGateway.Polling.Triggers;


public sealed class FixedIntervalTrigger : Lifecycle, IPollingTrigger
{
    private readonly ILogger _logger;
    private readonly TimeProvider _timeProvider;


    private readonly TimeSpan _interval;
    private PeriodicTimer? _periodicTimer;
    private CancellationTokenSource _tickTaskCts = new();
    private Task? _loopTask;

    private readonly ConcurrentDictionary<PollingTriggerOnTickHandler, bool> _callbacks = [];



    public FixedIntervalTrigger(TimeSpan interval, TimeProvider timeProvider, ILogger logger)
    {
        if (interval <= TimeSpan.Zero && interval != Timeout.InfiniteTimeSpan)
            throw new ArgumentOutOfRangeException(nameof(interval));

        ArgumentNullException.ThrowIfNull(timeProvider);
        ArgumentNullException.ThrowIfNull(logger);


        _interval = interval;
        _logger = logger ?? NullLogger.Instance;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }
    public FixedIntervalTrigger(TimeSpan interval, TimeProvider timeProvider) : this(interval, timeProvider, NullLogger.Instance) { }
    public FixedIntervalTrigger(TimeSpan interval) : this(interval, TimeProvider.System, NullLogger.Instance) { }



    public IDisposable Subscribe(PollingTriggerOnTickHandler onTick)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        _callbacks[onTick] = true;
        return new Disposer(() => _callbacks.TryRemove(onTick, out var _));
    }


    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        _tickTaskCts.Dispose();
        _tickTaskCts = new CancellationTokenSource();

        _periodicTimer?.Dispose();
        _periodicTimer = new PeriodicTimer(_interval, _timeProvider);

        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _loopTask = RunLoopAsync(started, _periodicTimer, _tickTaskCts.Token);
        await started.Task;
    }

    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        _periodicTimer?.Dispose();
        _periodicTimer = null;

        await _tickTaskCts.CancelAsync();
        await AwaitLoopAsync();
        _tickTaskCts.Dispose();
    }

    protected override async ValueTask OnDisposeAsync()
    {
        _periodicTimer?.Dispose();
        _periodicTimer = null;

        await _tickTaskCts.CancelAsync();
        if (_loopTask is not null)
        {
            try { await _loopTask; } catch (OperationCanceledException) { }
            _loopTask = null;
        }
        _tickTaskCts.Dispose();
    }


    private async Task AwaitLoopAsync()
    {
        var loop = Interlocked.Exchange(ref _loopTask, null);
        if (loop is null) return;
        try { await loop; } catch (OperationCanceledException) { }
    }

    private async Task RunLoopAsync(TaskCompletionSource started, PeriodicTimer timer, CancellationToken ct)
    {
        try
        {
            started.TrySetResult();
            while (await timer.WaitForNextTickAsync(ct))
                await OnTickAsync(ct);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) { _logger.LogError(ex, "轮询循环异常"); }
    }

    private async Task OnTickAsync(CancellationToken cancellationToken)
    {
        try
        {
            foreach (var i in _callbacks)
            {
                try { await i.Key.Invoke(cancellationToken); }
                catch (OperationCanceledException) { }
                catch (Exception ex) { _logger.LogError(ex, "轮询回调执行失败"); }
            }
        }
        catch (Exception ex) { _logger.LogError(ex, "tick 执行异常"); }
    }
}
