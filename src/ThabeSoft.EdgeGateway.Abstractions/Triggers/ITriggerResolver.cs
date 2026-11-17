namespace ThabeSoft.EdgeGateway.Triggers;


public interface ITriggerResolver
{
    bool CanResolve(TriggerDescriptor triggerDescriptor);
    ITrigger Resolve(TriggerDescriptor triggerDescriptor);
}