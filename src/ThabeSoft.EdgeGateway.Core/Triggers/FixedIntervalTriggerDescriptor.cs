namespace ThabeSoft.EdgeGateway.Triggers;

public sealed class FixedIntervalTriggerDescriptor : ITriggerDescriptor
{
    private FixedIntervalTriggerDescriptor() { }
    public required TimeSpan Interval { get; init; }


    public static FixedIntervalTriggerDescriptor From(TimeSpan timeSpan)
    {
        if (timeSpan <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(timeSpan), timeSpan, "间隔时间必须大于0");

        return new() { Interval = timeSpan };
    }

    public static FixedIntervalTriggerDescriptor FromMilliseconds(double milliseconds) =>
        From(TimeSpan.FromMilliseconds(milliseconds));
}