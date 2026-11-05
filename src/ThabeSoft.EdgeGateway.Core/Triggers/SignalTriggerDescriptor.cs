namespace ThabeSoft.EdgeGateway.Triggers;

public sealed class SignalTriggerDescriptor : ITriggerDescriptor
{
    private SignalTriggerDescriptor() { }
    public static SignalTriggerDescriptor Instance { get; } = new();
}