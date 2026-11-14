namespace ThabeSoft.EdgeGateway.Triggers;

public interface ITriggerFactory
{
    bool CanCreate(TriggerDescriptor triggerDescriptor);
    ITrigger Create(TriggerDescriptor triggerDescriptor);
}