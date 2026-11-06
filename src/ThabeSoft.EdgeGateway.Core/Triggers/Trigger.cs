namespace ThabeSoft.EdgeGateway.Triggers;


public static class Trigger
{
    public static ITriggerDescriptor Signal(out ISignal signal)
    {
        var descriptor = new SignalTriggerDescriptor();
        signal = descriptor;
        return descriptor;
    }

    public static ITriggerDescriptor Interval(TimeSpan timeSpan) =>
        FixedIntervalTriggerDescriptor.Create(timeSpan);
    public static ITriggerDescriptor Interval(double milliseconds) =>
        Interval(TimeSpan.FromMilliseconds(milliseconds));
}