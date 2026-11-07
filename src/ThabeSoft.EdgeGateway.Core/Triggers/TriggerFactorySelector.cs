namespace ThabeSoft.EdgeGateway.Triggers;


internal sealed class TriggerFactorySelector(IEnumerable<ITriggerFactory> factories)
{
    public ITrigger Create(ITriggerDescriptor triggerDescriptor)
    {
        return Select(triggerDescriptor).Create(triggerDescriptor);
    }
    public ITriggerFactory Select(ITriggerDescriptor triggerDescriptor)
    {
        return factories.FirstOrDefault(x => x.CanCreate(triggerDescriptor))
            ?? throw new InvalidOperationException();
    }
}