namespace ThabeSoft.EdgeGateway.Triggers;


internal sealed class FixedIntervalTriggerDescriptor : ITriggerDescriptor
{
    public required TimeSpan Interval { get; init; }

    private FixedIntervalTriggerDescriptor() { }
    public override string ToString() => $"FixedInterval({Interval.TotalMilliseconds}ms)";
    public bool Equals(ITriggerDescriptor other) => 
        other is FixedIntervalTriggerDescriptor @fixed && @fixed.Interval == Interval;


    public static FixedIntervalTriggerDescriptor Create(TimeSpan timeSpan)
    {
        if (timeSpan <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(timeSpan), timeSpan, "间隔时间必须大于0");

        return new FixedIntervalTriggerDescriptor() { Interval = timeSpan };
    }
}