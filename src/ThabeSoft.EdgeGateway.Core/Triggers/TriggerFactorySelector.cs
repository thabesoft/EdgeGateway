namespace ThabeSoft.EdgeGateway.Triggers;


internal sealed class TriggerFactorySelector(IEnumerable<ITriggerFactory> factories) : ITriggerFactorySelector
{
    public ITriggerFactory Select(ITriggerDescriptor triggerDescriptor)
    {
        return factories.FirstOrDefault(x => x.CanCreate(triggerDescriptor))
            ?? throw new InvalidOperationException();
    }
}