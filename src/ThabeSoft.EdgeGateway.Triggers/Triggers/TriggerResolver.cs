namespace ThabeSoft.EdgeGateway.Triggers;


internal sealed class TriggerResolver(IEnumerable<ITriggerFactory> factories) : ITriggerResolver
{
    public ITrigger Resolve(TriggerDescriptor triggerDescriptor)
    {
        var factory = factories.FirstOrDefault(x => x.CanCreate(triggerDescriptor))
            ?? throw new InvalidOperationException();

        return factory.Create(triggerDescriptor);
    }
}