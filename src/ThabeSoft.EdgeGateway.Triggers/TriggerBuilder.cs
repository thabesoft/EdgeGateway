using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.EdgeGateway.Triggers.Interval;
using ThabeSoft.EdgeGateway.Triggers.Signal;

namespace ThabeSoft.EdgeGateway;


public static class TriggerBuilder
{
    public static TriggerDescriptor Signal(out ISignal signal)
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