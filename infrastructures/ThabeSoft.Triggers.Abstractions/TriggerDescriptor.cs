namespace ThabeSoft.EdgeGateway.Triggers;


public abstract record class TriggerDescriptor
{
    public abstract TriggerType Type { get; }
}