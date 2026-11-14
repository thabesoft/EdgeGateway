namespace ThabeSoft.EdgeGateway.Triggers;


public interface ITriggerResolver
{
    ITrigger Resolve(TriggerDescriptor triggerDescriptor);
}