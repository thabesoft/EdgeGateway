using System.Collections.Concurrent;

namespace ThabeSoft.EdgeGateway.Triggers;


internal sealed class TriggerManager(IEnumerable<ITriggerFactory> triggerFactories)
{
    private readonly ConcurrentDictionary<ITrigger, byte> _entries = [];
    public IReadOnlyCollection<ITrigger> Triggers => [.. _entries.Keys];


    public async ValueTask<ITrigger> RegisterAsync(ITriggerDescriptor triggerDescriptor)
    {
        var trigger = triggerFactories.FirstOrDefault(x => x.CanCreate(triggerDescriptor))
            ?.Create(triggerDescriptor);
        ArgumentNullException.ThrowIfNull(trigger);


        var entry = new Entry(trigger, this);
        if (_entries.TryAdd(entry, 0)) return entry;

        await entry.DisposeAsync();
        throw new InvalidOperationException();
    }


    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }


    private sealed class Entry(ITrigger trigger, TriggerManager manager) : ITrigger
    {
        public async ValueTask DisposeAsync()
        {
            await trigger.DisposeAsync();

            manager._entries.TryRemove(trigger, out _);
        }

        public ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default) =>
            trigger.WaitForNextTickAsync(cancellationToken);
    }
}
