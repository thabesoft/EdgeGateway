namespace ThabeSoft.EdgeGateway.Triggers.Interval;


internal sealed class FixedIntervalTriggerFactory(TimeProvider timeProvider) : ITriggerFactory
{
    public bool CanCreate(ITriggerDescriptor triggerDescriptor) =>
        triggerDescriptor is FixedIntervalTriggerDescriptor;

    public ITrigger Create(ITriggerDescriptor triggerDescriptor)
    {
        if (triggerDescriptor is not FixedIntervalTriggerDescriptor descriptor)
            throw new InvalidOperationException($"不支持的触发器描述: {triggerDescriptor.GetType()}");

        return new FixedIntervalTrigger(descriptor, timeProvider);
    }
}
