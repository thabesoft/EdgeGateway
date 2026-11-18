using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Triggers.Interval;


public sealed record class IntervalTriggerDescriptor : TriggerDescriptor
{
    public required TimeSpan Interval { get; init => field = value.Positive(); }
}