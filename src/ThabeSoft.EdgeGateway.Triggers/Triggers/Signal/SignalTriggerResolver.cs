using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Triggers.Signal;

internal sealed class SignalTriggerResolver : ITriggerResolver
{
    public bool CanResolve(TriggerDescriptor descriptor) =>
        descriptor is SignalTriggerDescriptor;

    public ITrigger Resolve(TriggerDescriptor descriptor)
    {
        var signal = descriptor.AssertType<SignalTriggerDescriptor>();
        return new SignalTrigger(signal);
    }
}
