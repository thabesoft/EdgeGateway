namespace ThabeSoft.Triggers.Interval;


internal sealed class IntervalTrigger(IntervalTriggerDescriptor descriptor, TimeProvider timeProvider) : ITrigger
{ 
    private readonly PeriodicTimer _periodicTimer = new(descriptor.Interval, timeProvider);

    public IntervalTrigger(IntervalTriggerDescriptor descriptor) : this(descriptor, TimeProvider.System) { }

    public ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken) =>
        _periodicTimer.WaitForNextTickAsync(cancellationToken);

    public ValueTask DisposeAsync()
    {
        _periodicTimer.Dispose();
        return ValueTask.CompletedTask;
    }
}
