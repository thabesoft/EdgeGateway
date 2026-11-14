using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace ThabeSoft.EdgeGateway.Triggers;


internal sealed class TriggerScheduler(ITriggerResolver triggerFactorySelector, ILoggerFactory loggerFactory) : ITriggerScheduler
{
    private readonly ConcurrentDictionary<ITriggerDescriptor, TriggerBatchPublisher> _ticks = [];

    public IDisposable Register(ITriggerDescriptor triggerDescriptor, Func<CancellationToken, ValueTask> onTickAsync)
    {
        var method = _ticks.GetOrAdd(triggerDescriptor, x =>
        {
            var trigger = triggerFactorySelector.Resolve(x);
            var logger = loggerFactory.CreateLogger<TriggerBatchPublisher>();

            return new TriggerBatchPublisher(trigger, logger);
        });

        return method.Subcribe(onTickAsync);
    }
}

internal sealed class TriggerBatchPublisher : IAsyncDisposable
{
    private readonly object _lock = new();
    private readonly ConcurrentDictionary<Guid, Func<CancellationToken, ValueTask>> _actions = [];
    private readonly ITrigger _trigger;
    private readonly ILogger<TriggerBatchPublisher> _logger;
    private Func<CancellationToken, ValueTask>[] _snapshot = [];
    private readonly Task _loopTask;
    private readonly CancellationTokenSource _loopTaskCts = new();


    public TriggerBatchPublisher(ITrigger trigger, ILogger<TriggerBatchPublisher> logger)
    {
        _trigger = trigger;
        _logger = logger;
        _loopTask = LoopAsync(_loopTaskCts.Token);
    }

    public IDisposable Subcribe(Func<CancellationToken, ValueTask> onTick)
    {
        var id = Guid.NewGuid();
        lock (_lock)
        {
            _actions[id] = onTick;
            _snapshot = [.. _actions.Values];
        }

        return new Releaser(() =>
        {
            lock (_lock)
            {
                if (_actions.TryRemove(id, out _))
                    _snapshot = [.. _actions.Values];
            }
        });
    }

    private async Task LoopAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!await _trigger.WaitForNextTickAsync(cancellationToken))
                return;

            foreach (var i in _snapshot)
                await i.Invoke(cancellationToken);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _loopTaskCts.CancelAsync();
        try
        {
            await _loopTask;
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "释放");
        }
    }

    private sealed class Releaser(Action action) : IDisposable
    {
        private int _disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0) action();
        }
    }
}