namespace ThabeSoft.EdgeGateway.Triggers;

public interface ITriggerScheduler
{
    IDisposable Register(TriggerDescriptor triggerDescriptor, Func<CancellationToken, ValueTask> onTickAsync);
}
