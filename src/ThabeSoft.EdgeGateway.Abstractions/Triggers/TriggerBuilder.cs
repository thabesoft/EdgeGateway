using ThabeSoft.EdgeGateway.Triggers.Interval;
using ThabeSoft.EdgeGateway.Triggers.Signal;

namespace ThabeSoft.EdgeGateway.Triggers;


public static class TriggerBuilder
{
    public static TriggerDescriptor Signal(out ITriggerSignal signal)
    {
        var descriptor = new SignalTriggerDescriptor();
        signal = descriptor;
        return descriptor;
    }

    public static TriggerDescriptor Interval(TimeSpan timeSpan) =>
        new IntervalTriggerDescriptor() { Interval = timeSpan };
    public static TriggerDescriptor Interval(double milliseconds) =>
        Interval(TimeSpan.FromMilliseconds(milliseconds));
}