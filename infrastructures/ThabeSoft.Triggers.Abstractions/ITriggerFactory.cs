namespace ThabeSoft.Triggers;

public interface ITriggerFactory
{
    ITrigger Create(TriggerDescriptor triggerDescriptor);
}