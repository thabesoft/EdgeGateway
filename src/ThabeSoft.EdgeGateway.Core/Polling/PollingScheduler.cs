using ThabeSoft.EdgeGateway.Polling.Triggers;
using ThabeSoft.EdgeGateway.Tag;
using System.Collections.Concurrent;

namespace ThabeSoft.EdgeGateway.Polling;


internal sealed class PollingScheduler(ITagReader tagReader) : Lifecycle, IPollingScheduler
{
    private readonly ConcurrentDictionary<TagName, Context> _context = [];
    public IReadOnlyDictionary<TagName, ITagMonitor> Monitors => _context.ToDictionary(k => k.Key, v => v.Value.Monitor);



    public ITagMonitor<T> Register<T>(TagName tagName, ITag<T> tag, IPollingTrigger pollingTrigger) where T : notnull
    {
        var monitor = new TagMonitor<T>(tag);
        var disposer = pollingTrigger.Subscribe(async ct =>
        {
            var value = await tagReader.ReadAsync(tag, ct);
            monitor.Update(value);
        });

        _context[tagName] = new(disposer)
        {
            Monitor = monitor,
            Trigger = pollingTrigger
        };
        return monitor;
    }
    public void Unregister(TagName tagName)
    {
        if (!_context.TryRemove(tagName, out var context))
            return;

        context.Dispose();
    }


    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        foreach (var context in _context)
        {
            await context.Value.Trigger.StartAsync(cancellationToken);
        }
    }

    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        foreach (var context in _context)
        {
            await context.Value.Trigger.StopAsync(cancellationToken);
        }
    }
    protected override ValueTask OnDisposeAsync()
    {
        var contexts = _context.Values.ToArray();
        _context.Clear();

        foreach (var i in contexts)
        {
            i.Dispose();
        }

        return ValueTask.CompletedTask;
    }

    private class Context(IDisposable disposer) : IDisposable
    {
        public required ITagMonitor Monitor { get; init; }
        public required IPollingTrigger Trigger { get; init; }

        public void Dispose() => disposer.Dispose();
    }
}