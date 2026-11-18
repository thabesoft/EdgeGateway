using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Triggers.Interval;


public sealed record class IntervalTriggerDescriptor : TriggerDescriptor
{
    public override TriggerType Type => TriggerType.Interval;
    public required TimeSpan Interval { get; init => field = value.Positive(); }
}