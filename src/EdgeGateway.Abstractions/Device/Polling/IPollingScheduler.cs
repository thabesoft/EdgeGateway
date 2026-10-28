using EdgeGateway.Tag;
using System.Collections.Concurrent;

namespace EdgeGateway.Device.Polling;


public interface IPollingScheduler
{
    IReadOnlyDictionary<ITag, ITagMonitor> Monitors { get; }
    ITagMonitor<T> Register<T>(ITag<T> tag, IPollingTrigger pollingTrigger) where T : notnull;
    void Unregister<T>(ITag<T> tag) where T : notnull;


    ValueTask StartAsync(CancellationToken cancellationToken = default);
    ValueTask StopAsync(CancellationToken cancellationToken = default);
}


public sealed class PollingScheduler(ITagReader tagReader) : IPollingScheduler, IDisposable
{
    private readonly ConcurrentDictionary<ITag, Context> _context = [];
    public IReadOnlyDictionary<ITag, ITagMonitor> Monitors => _context.ToDictionary(k => k.Key, v => v.Value.Monitor);



    public async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        foreach (var context in _context)
        {
            await context.Value.Trigger.StartAsync(cancellationToken);
        }
    }

    public async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        foreach (var context in _context)
        {
            await context.Value.Trigger.StopAsync(cancellationToken);
        }
    }


    public ITagMonitor<T> Register<T>(ITag<T> tag, IPollingTrigger pollingTrigger) where T : notnull
    {
        var monitor = new TagMonitor<T>(tag);
        var disposer =  pollingTrigger.Subscribe(async ct =>
        {
            var value = await tagReader.ReadAsync(tag, ct);
            monitor.Update(value);
        });

        _context[tag] = new(disposer)
        {
            Monitor = monitor,
            Trigger = pollingTrigger
        };
        return monitor;
    }

    public void Unregister<T>(ITag<T> tag) where T : notnull
    {
        if (!_context.TryRemove(tag, out var context))
            return;

        context.Dispose();
    }

    public void Dispose()
    {
        var contexts = _context.Values.ToArray();
        _context.Clear();

        foreach (var i in contexts)
        {
            i.Dispose();
        }
    }

    private class Context(IDisposable disposer) : IDisposable
    {
        public required ITagMonitor Monitor { get; init; }
        public required IPollingTrigger Trigger { get; init; }

        public void Dispose() => disposer.Dispose();
    }
}