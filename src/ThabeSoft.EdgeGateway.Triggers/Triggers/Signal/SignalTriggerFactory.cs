namespace ThabeSoft.EdgeGateway.Triggers.Signal;

internal sealed class SignalTriggerFactory : ITriggerFactory
{
    public bool CanCreate(TriggerDescriptor triggerDescriptor) =>
        triggerDescriptor is SignalTriggerDescriptor;

    public ITrigger Create(TriggerDescriptor triggerDescriptor)
    {
        if (triggerDescriptor is not SignalTriggerDescriptor descriptor)
            throw new InvalidOperationException($"不支持的触发器描述: {triggerDescriptor.GetType()}");

        return new SignalTrigger(descriptor);
    }
}
