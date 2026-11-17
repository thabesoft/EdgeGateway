namespace ThabeSoft.EdgeGateway.Triggers;

public interface ITriggerFactory
{
    ITrigger Create(TriggerDescriptor triggerDescriptor);
}