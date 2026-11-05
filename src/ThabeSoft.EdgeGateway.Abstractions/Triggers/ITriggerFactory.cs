namespace ThabeSoft.EdgeGateway.Triggers;

public interface ITriggerFactory
{
    bool CanCreate(ITriggerDescriptor triggerDescriptor);
    ITrigger Create(ITriggerDescriptor triggerDescriptor);
}