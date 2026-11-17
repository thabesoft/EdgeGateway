namespace ThabeSoft.EdgeGateway.Triggers;


internal sealed class TriggerFactory(IEnumerable<ITriggerResolver> resolvers) : ITriggerFactory
{
    public ITrigger Create(TriggerDescriptor triggerDescriptor)
    {
        ArgumentNullException.ThrowIfNull(triggerDescriptor);

        var factory = resolvers.FirstOrDefault(x => x.CanResolve(triggerDescriptor))
            ?? throw new NotSupportedTriggerException(triggerDescriptor);

        return factory.Resolve(triggerDescriptor);
    }
}

public sealed class NotSupportedTriggerException(TriggerDescriptor trigger) :
    NotSupportedException($"不支持的触发器 [{trigger.GetType()}]: {trigger}")
{
    public TriggerDescriptor Trigger { get; } = trigger ?? throw new ArgumentNullException(nameof(trigger));
}