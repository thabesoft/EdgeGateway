namespace ThabeSoft.EdgeGateway.Triggers;


public interface ITriggerResolver
{
    ITrigger Resolve(ITriggerDescriptor triggerDescriptor);
}