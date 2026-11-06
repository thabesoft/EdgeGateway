namespace ThabeSoft.EdgeGateway.Triggers;


public static class TriggerDescriptor
{
    public static ITriggerDescriptor Signal { get; } = new SignalTriggerDescriptor();

    public static ITriggerDescriptor Interval(TimeSpan timeSpan)
    {
        if (timeSpan <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(timeSpan), timeSpan, "间隔时间必须大于0");

        return new FixedIntervalTriggerDescriptor() { Interval = timeSpan };
    }

    public static ITriggerDescriptor Interval(double milliseconds) =>
        Interval(TimeSpan.FromMilliseconds(milliseconds));
}