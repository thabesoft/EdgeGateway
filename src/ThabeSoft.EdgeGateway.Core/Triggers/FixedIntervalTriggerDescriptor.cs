namespace ThabeSoft.EdgeGateway.Triggers;


internal sealed class FixedIntervalTriggerDescriptor : ITriggerDescriptor
{
    public required TimeSpan Interval { get; init; }

    private FixedIntervalTriggerDescriptor() { }
    public static FixedIntervalTriggerDescriptor Create(TimeSpan timeSpan)
    {
        if (timeSpan <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(timeSpan), timeSpan, "间隔时间必须大于0");

        return new FixedIntervalTriggerDescriptor() { Interval = timeSpan };
    }
}