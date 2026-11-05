namespace ThabeSoft.EdgeGateway.Triggers;


internal sealed class FixedIntervalTrigger(FixedIntervalTriggerDescriptor descriptor, TimeProvider timeProvider) : ITrigger
{
    private readonly PeriodicTimer _periodicTimer = new(descriptor.Interval, timeProvider);

    public FixedIntervalTrigger(FixedIntervalTriggerDescriptor descriptor) : this(descriptor, TimeProvider.System) { }

    public ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken) =>
        _periodicTimer.WaitForNextTickAsync(cancellationToken);

    public ValueTask DisposeAsync()
    {
        _periodicTimer.Dispose();
        return ValueTask.CompletedTask;
    }
}
