using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Triggers.Interval;


internal sealed class IntervalTriggerResolver(
        TimeProvider timeProvider
    ) : ITriggerResolver
{
    public bool CanResolve(TriggerDescriptor triggerDescriptor) =>
        triggerDescriptor is IntervalTriggerDescriptor;

    public ITrigger Resolve(TriggerDescriptor triggerDescriptor)
    {
        var interval = triggerDescriptor.AssertType<IntervalTriggerDescriptor>();
        return new IntervalTrigger(interval, timeProvider);
    }
}