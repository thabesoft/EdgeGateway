using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Utilities;


internal sealed class TriggerScheduler : ILifecycle
{
    private readonly ConcurrentDictionary<Guid, Action> _entries = [];
    private readonly ITrigger trigger;
    private readonly ILogger<TriggerScheduler> _logger;
    private readonly IBackgroundLifecycle _background;

    public TriggerScheduler(ITrigger trigger, ILogger<TriggerScheduler> logger)
    {
        this.trigger = trigger;
        _logger = logger;
        _background = Lifecycle.Background(LoopAsync, logger);
    }

    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _background.StartAsync(cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _background.StopAsync(cancellationToken);

    public IDisposable Subcribe(Action action)
    {
        var id = Guid.NewGuid();
        _entries[id] = action;

        return new Releaser(() => _entries.TryRemove(id, out _));
    }

    private async Task LoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!await trigger.WaitForNextTickAsync(cancellationToken))
                    return;

                _entries.Values.AsParallel().ForAll(x => x.Invoke());
            }
            catch(OperationCanceledException)
            {

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "触发器循环异常");
            }
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