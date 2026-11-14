using ThabeSoft.EdgeGateway.Compatibility;

namespace ThabeSoft.EdgeGateway.Triggers.Interval;


public sealed record class FixedIntervalTriggerDescriptor : TriggerDescriptor
{
    public required TimeSpan Interval { get; init => field = value.Positive(); }
}