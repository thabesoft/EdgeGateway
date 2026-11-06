using System.Collections.Concurrent;

namespace ThabeSoft.EdgeGateway.Triggers;


internal sealed class TriggerManager(IEnumerable<ITriggerFactory> factories)
{
    private readonly ConcurrentDictionary<ITrigger, byte> _entries = [];
    public IReadOnlyCollection<ITrigger> Triggers => [.. _entries.Keys];


    public ITrigger Register(ITriggerDescriptor descriptor)
    {
        var trigger = factories.FirstOrDefault(x => x.CanCreate(descriptor))
            ?.Create(descriptor);
        ArgumentNullException.ThrowIfNull(trigger);


        var entry = new Entry(trigger, this);
        if (_entries.TryAdd(entry, 0)) return entry;

        entry.Dispose();
        throw new InvalidOperationException();
    }


    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }


    private sealed class Entry(
            ITrigger trigger,
            TriggerManager manager
        ) : ITrigger, IDisposable
    {
        private int _disposed;

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

            await trigger.DisposeAsync();
            manager._entries.TryRemove(trigger, out _);
        }
        public async void Dispose()
        {
            await DisposeAsync();
        }


        public ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default) =>
            trigger.WaitForNextTickAsync(cancellationToken);
    }
}
