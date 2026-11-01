namespace ThabeSoft.EdgeGateway.Triggers;


public sealed class SignalTrigger : ITrigger
{
    private HashSet<TriggerHandler> _handlers = [];
    private readonly object _lock = new();


    public IDisposable Subscribe(TriggerHandler onTick)
    {
        ArgumentNullException.ThrowIfNull(onTick);

        lock (_lock)
        {
            _handlers.Add(onTick);
        }

        return new Disposer(() =>
        {
            lock (_lock) { _handlers.Remove(onTick); }
        });
    }

    public async ValueTask OnTickAsync(CancellationToken cancellationToken = default)
    {
        TriggerHandler[] snapshot;
        lock (_lock)
        {
            snapshot = [.. _handlers];
        }

        foreach (var handler in snapshot)
        {
            await handler.Invoke(cancellationToken);
        }
    }
}