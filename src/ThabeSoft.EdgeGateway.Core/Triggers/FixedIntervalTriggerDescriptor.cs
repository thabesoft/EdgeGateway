namespace ThabeSoft.EdgeGateway.Triggers;


internal sealed class FixedIntervalTriggerDescriptor : ITriggerDescriptor
{
    internal FixedIntervalTriggerDescriptor() { }
    public required TimeSpan Interval { get; init; }
}