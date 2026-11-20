namespace ThabeSoft.EdgeGateway.Triggers;


public interface ITriggerResolver
{
    bool CanResolve(TriggerDescriptor descriptor);
    ITrigger Resolve(TriggerDescriptor descriptor);
}