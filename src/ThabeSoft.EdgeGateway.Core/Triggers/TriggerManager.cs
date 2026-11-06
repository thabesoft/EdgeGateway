using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace ThabeSoft.EdgeGateway.Triggers;


internal sealed class TriggerManager(
        IEnumerable<ITriggerFactory> factories, 
        ILoggerFactory loggerFactory
    ) : IAsyncDisposable
{
    private readonly ILogger<TriggerManager> _logger = loggerFactory.CreateLogger<TriggerManager>();
    private readonly ConcurrentDictionary<Guid, ITrigger> _entries = [];


    public ITrigger Register(ITriggerDescriptor descriptor)
    {
        var trigger = factories.FirstOrDefault(x => x.CanCreate(descriptor))
            ?.Create(descriptor);
        ArgumentNullException.ThrowIfNull(trigger);


        var id = Guid.NewGuid();
        var entry = new Entry(id, trigger, this, _logger);
        if (_entries.TryAdd(id, entry)) return entry;

        entry.Dispose();
        throw new InvalidOperationException();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var i in _entries.Values)
            await i.DisposeAsync();

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("触发器理器已释放");
    }


    private sealed class Entry(
            Guid id,
            ITrigger trigger,
            TriggerManager manager,
            ILogger logger
        ) : ITrigger, IDisposable
    {
        private int _disposed;

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

            await trigger.DisposeAsync();
            manager._entries.TryRemove(id, out _);

            if (logger.IsEnabled(LogLevel.Trace))
                logger.LogTrace("触发器 [{Endpoint}] 释放", trigger.GetType().Name);
        }
        public async void Dispose()
        {
            await DisposeAsync();
        }


        public async ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
        {
            var result = await trigger.WaitForNextTickAsync(cancellationToken);

            if (logger.IsEnabled(LogLevel.Trace))
                logger.LogTrace("触发器 [{Name}] 触发", trigger.GetType().Name);

            return result;
        }
    }
}
